using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using spatc.Models;

namespace spatc.Controllers.Admin
{
    [Authorize]
    [Route("Admin/[controller]")]
    public class HoaDonController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HoaDonController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/HoaDon
        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var hoaDons = await _context.HoaDons
                .Include(h => h.DatLich)
                .ThenInclude(d => d.ThuCung)
                .ThenInclude(t => t.KhachHang)
                .Include(h => h.DatLich)
                .ThenInclude(d => d.DichVu)
                .OrderByDescending(h => h.NgayThanhToan)
                .ToListAsync();
            return View("~/Views/Admin/HoaDon/Index.cshtml", hoaDons);
        }

        // GET: Admin/HoaDon/Details/5
        [HttpGet]
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons
                .Include(h => h.DatLich)
                .ThenInclude(d => d.ThuCung)
                .ThenInclude(t => t.KhachHang)
                .Include(h => h.DatLich)
                .ThenInclude(d => d.DichVu)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (hoaDon == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/HoaDon/Details.cshtml", hoaDon);
        }

        // GET: Admin/HoaDon/Create
        [HttpGet]
        [Route("Create")]
        public async Task<IActionResult> Create(int? datLichId, string? loaiHoaDon)
        {
            // Load danh sách đặt lịch đã hoàn thành và chưa có hóa đơn
            var datLiches = await _context.DatLiches
                .Include(d => d.ThuCung)
                .ThenInclude(t => t.KhachHang)
                .Include(d => d.DichVu)
                .Where(d => d.TrangThai == "Hoàn thành" && d.HoaDon == null)
                .ToListAsync();

            ViewData["DatLichId"] = new SelectList(datLiches, "Id", "Id", datLichId);
            
            // Load danh sách dịch vụ Grooming và Khách sạn cho walk-in từ bảng DichVus (đồng bộ với trang quản lý)
            var groomingServices = await _context.DichVus
                .Where(d => d.TrangThai == true && d.LoaiDichVu == "Grooming")
                .OrderBy(d => d.TenDichVu)
                .ToListAsync();

            var phongKhachSans = await _context.DichVus
                .Where(d => d.TrangThai == true && d.LoaiDichVu == "KhachSan")
                .OrderBy(d => d.TenDichVu)
                .ToListAsync();

            ViewBag.GroomingServices = groomingServices;
            ViewBag.PhongKhachSans = phongKhachSans;
            ViewBag.LoaiHoaDon = loaiHoaDon ?? "TuDatLich"; // Mặc định là "Từ đặt lịch"
            
            if (datLichId.HasValue)
            {
                var datLich = await _context.DatLiches
                    .Include(d => d.DichVu)
                    .FirstOrDefaultAsync(d => d.Id == datLichId);
                if (datLich != null)
                {
                    ViewBag.DichVuGia = datLich.DichVu?.Gia ?? 0;
                }
            }

            return View("~/Views/Admin/HoaDon/Create.cshtml");
        }

        // POST: Admin/HoaDon/Create
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("DatLichId,TongTien,PhuongThucThanhToan,GhiChu,LoaiHoaDon,KhachHangTen,KhachHangSoDienThoai,KhachHangEmail,TenDichVu,LoaiDichVu")] HoaDon hoaDon,
            int? DichVuGroomingId,
            int? PhongKhachSanId)
        {
            // Validate dựa trên loại hóa đơn
            if (hoaDon.LoaiHoaDon == "TuDatLich")
            {
                // Hóa đơn từ đặt lịch: cần DatLichId
                if (!hoaDon.DatLichId.HasValue)
                {
                    ModelState.AddModelError("DatLichId", "Vui lòng chọn đặt lịch!");
                }
            }
            else if (hoaDon.LoaiHoaDon == "KhachDenQuan")
            {
                // Hóa đơn cho khách đến quán: cần thông tin khách hàng và dịch vụ
                if (string.IsNullOrWhiteSpace(hoaDon.KhachHangTen))
                {
                    ModelState.AddModelError("KhachHangTen", "Vui lòng nhập tên khách hàng!");
                }
                if (string.IsNullOrWhiteSpace(hoaDon.KhachHangSoDienThoai))
                {
                    ModelState.AddModelError("KhachHangSoDienThoai", "Vui lòng nhập số điện thoại!");
                }
                if (hoaDon.TongTien <= 0)
                {
                    ModelState.AddModelError("TongTien", "Tổng tiền phải lớn hơn 0!");
                }

                // Xác định dịch vụ được chọn (lấy từ bảng DichVus để đồng bộ với trang quản lý)
                if (DichVuGroomingId.HasValue)
                {
                    var groomingService = await _context.DichVus
                        .Where(d => d.Id == DichVuGroomingId.Value && d.LoaiDichVu == "Grooming" && d.TrangThai == true)
                        .FirstOrDefaultAsync();
                    if (groomingService != null)
                    {
                        hoaDon.TenDichVu = groomingService.TenDichVu;
                        hoaDon.LoaiDichVu = "Grooming";
                        if (hoaDon.TongTien == 0)
                        {
                            hoaDon.TongTien = groomingService.Gia;
                        }
                    }
                }
                else if (PhongKhachSanId.HasValue)
                {
                    var phong = await _context.DichVus
                        .Where(d => d.Id == PhongKhachSanId.Value && d.LoaiDichVu == "KhachSan" && d.TrangThai == true)
                        .FirstOrDefaultAsync();
                    if (phong != null)
                    {
                        hoaDon.TenDichVu = phong.TenDichVu;
                        hoaDon.LoaiDichVu = "KhachSan";
                        if (hoaDon.TongTien == 0)
                        {
                            hoaDon.TongTien = phong.Gia; // Sử dụng giá từ DichVu
                        }
                    }
                }
                else if (string.IsNullOrWhiteSpace(hoaDon.TenDichVu))
                {
                    ModelState.AddModelError("", "Vui lòng chọn dịch vụ hoặc nhập tên dịch vụ!");
                }
            }

            if (ModelState.IsValid)
            {
                hoaDon.NgayThanhToan = DateTime.Now;
                hoaDon.NgayTao = DateTime.Now;
                
                // Đảm bảo LoaiHoaDon có giá trị
                if (string.IsNullOrWhiteSpace(hoaDon.LoaiHoaDon))
                {
                    hoaDon.LoaiHoaDon = hoaDon.DatLichId.HasValue ? "TuDatLich" : "KhachDenQuan";
                }

                _context.Add(hoaDon);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Tạo hóa đơn thành công!";
                return RedirectToAction(nameof(Index));
            }
            
            // Reload dữ liệu cho view nếu có lỗi (lấy từ DichVus để đồng bộ)
            var datLiches = await _context.DatLiches
                .Include(d => d.ThuCung)
                .ThenInclude(t => t.KhachHang)
                .Include(d => d.DichVu)
                .Where(d => d.TrangThai == "Hoàn thành" && d.HoaDon == null)
                .ToListAsync();

            var groomingServices = await _context.DichVus
                .Where(d => d.TrangThai == true && d.LoaiDichVu == "Grooming")
                .OrderBy(d => d.TenDichVu)
                .ToListAsync();

            var phongKhachSans = await _context.DichVus
                .Where(d => d.TrangThai == true && d.LoaiDichVu == "KhachSan")
                .OrderBy(d => d.TenDichVu)
                .ToListAsync();

            ViewData["DatLichId"] = new SelectList(datLiches, "Id", "Id", hoaDon.DatLichId);
            ViewBag.GroomingServices = groomingServices;
            ViewBag.PhongKhachSans = phongKhachSans;
            ViewBag.LoaiHoaDon = hoaDon.LoaiHoaDon;
            ViewBag.SelectedGroomingId = DichVuGroomingId;
            ViewBag.SelectedPhongId = PhongKhachSanId;
            
            return View("~/Views/Admin/HoaDon/Create.cshtml", hoaDon);
        }

        // GET: Admin/HoaDon/Delete/5
        [HttpGet]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons
                .Include(h => h.DatLich)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (hoaDon == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/HoaDon/Delete.cshtml", hoaDon);
        }

        // POST: Admin/HoaDon/Delete/5
        [HttpPost]
        [Route("Delete/{id}")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon != null)
            {
                _context.HoaDons.Remove(hoaDon);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa hóa đơn thành công!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

