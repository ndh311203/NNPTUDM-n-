using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using spatc.Models;

namespace spatc.Controllers
{
    [Authorize]
    public class MyPetController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MyPetController> _logger;

        public MyPetController(ApplicationDbContext context, ILogger<MyPetController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: MyPet/Index
        [HttpGet]
        [Route("MyPet")]
        [Route("MyPet/Index")]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int accountId))
            {
                return RedirectToAction("Index", "Login");
            }

            // Tìm khách hàng liên kết với tài khoản
            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(k => k.TaiKhoanId == accountId);

            if (khachHang == null)
            {
                TempData["Error"] = "Vui lòng cập nhật thông tin cá nhân trước khi quản lý thú cưng!";
                return RedirectToAction("Profile", "Account");
            }

            // Lấy danh sách thú cưng của khách hàng
            var thuCungs = await _context.ThuCungs
                .Where(t => t.KhachHangId == khachHang.Id)
                .OrderByDescending(t => t.NgayTao)
                .ToListAsync();

            return View("~/Views/MyPet/Index.cshtml", thuCungs);
        }

        // GET: MyPet/Create
        [HttpGet]
        [Route("MyPet/Create")]
        public IActionResult Create()
        {
            return View("~/Views/MyPet/Create.cshtml");
        }

        // POST: MyPet/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("MyPet/Create")]
        public async Task<IActionResult> Create(
            string tenThuCung,
            string loai,
            string? giong,
            int? tuoi,
            decimal? canNang,
            string? ghiChuSucKhoe,
            string? hinhAnh)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int accountId))
            {
                return RedirectToAction("Index", "Login");
            }

            if (string.IsNullOrEmpty(tenThuCung) || string.IsNullOrEmpty(loai))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin bắt buộc!";
                return View("~/Views/MyPet/Create.cshtml");
            }

            // Tìm khách hàng liên kết với tài khoản
            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(k => k.TaiKhoanId == accountId);

            if (khachHang == null)
            {
                TempData["Error"] = "Vui lòng cập nhật thông tin cá nhân trước khi thêm thú cưng!";
                return RedirectToAction("Profile", "Account");
            }

            try
            {
                var thuCung = new ThuCung
                {
                    TenThuCung = tenThuCung,
                    Loai = loai,
                    Giong = giong,
                    Tuoi = tuoi,
                    CanNang = canNang,
                    GhiChuSucKhoe = ghiChuSucKhoe,
                    HinhAnh = hinhAnh,
                    KhachHangId = khachHang.Id,
                    NgayTao = DateTime.Now
                };

                _context.ThuCungs.Add(thuCung);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Thêm thú cưng thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm thú cưng");
                ViewBag.Error = "Đã xảy ra lỗi khi thêm thú cưng!";
                return View("~/Views/MyPet/Create.cshtml");
            }
        }

        // GET: MyPet/Edit/5
        [HttpGet]
        [Route("MyPet/Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int accountId))
            {
                return RedirectToAction("Index", "Login");
            }

            // Tìm khách hàng liên kết với tài khoản
            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(k => k.TaiKhoanId == accountId);

            if (khachHang == null)
            {
                return NotFound();
            }

            // Kiểm tra thú cưng thuộc về khách hàng này
            var thuCung = await _context.ThuCungs
                .FirstOrDefaultAsync(t => t.Id == id && t.KhachHangId == khachHang.Id);

            if (thuCung == null)
            {
                TempData["Error"] = "Không tìm thấy thú cưng hoặc bạn không có quyền chỉnh sửa!";
                return RedirectToAction("Index");
            }

            return View("~/Views/MyPet/Edit.cshtml", thuCung);
        }

        // POST: MyPet/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("MyPet/Edit/{id}")]
        public async Task<IActionResult> Edit(
            int id,
            string tenThuCung,
            string loai,
            string? giong,
            int? tuoi,
            decimal? canNang,
            string? ghiChuSucKhoe,
            string? hinhAnh)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int accountId))
            {
                return RedirectToAction("Index", "Login");
            }

            if (string.IsNullOrEmpty(tenThuCung) || string.IsNullOrEmpty(loai))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin bắt buộc!";
                var thuCungForView = await _context.ThuCungs.FindAsync(id);
                return View("~/Views/MyPet/Edit.cshtml", thuCungForView);
            }

            // Tìm khách hàng liên kết với tài khoản
            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(k => k.TaiKhoanId == accountId);

            if (khachHang == null)
            {
                return NotFound();
            }

            // Kiểm tra thú cưng thuộc về khách hàng này
            var thuCung = await _context.ThuCungs
                .FirstOrDefaultAsync(t => t.Id == id && t.KhachHangId == khachHang.Id);

            if (thuCung == null)
            {
                TempData["Error"] = "Không tìm thấy thú cưng hoặc bạn không có quyền chỉnh sửa!";
                return RedirectToAction("Index");
            }

            try
            {
                thuCung.TenThuCung = tenThuCung;
                thuCung.Loai = loai;
                thuCung.Giong = giong;
                thuCung.Tuoi = tuoi;
                thuCung.CanNang = canNang;
                thuCung.GhiChuSucKhoe = ghiChuSucKhoe;
                thuCung.HinhAnh = hinhAnh;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật thông tin thú cưng thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật thú cưng");
                ViewBag.Error = "Đã xảy ra lỗi khi cập nhật thông tin!";
                return View("~/Views/MyPet/Edit.cshtml", thuCung);
            }
        }

        // GET: MyPet/Delete/5
        [HttpGet]
        [Route("MyPet/Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int accountId))
            {
                return RedirectToAction("Index", "Login");
            }

            // Tìm khách hàng liên kết với tài khoản
            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(k => k.TaiKhoanId == accountId);

            if (khachHang == null)
            {
                return NotFound();
            }

            // Kiểm tra thú cưng thuộc về khách hàng này
            var thuCung = await _context.ThuCungs
                .FirstOrDefaultAsync(t => t.Id == id && t.KhachHangId == khachHang.Id);

            if (thuCung == null)
            {
                TempData["Error"] = "Không tìm thấy thú cưng hoặc bạn không có quyền xóa!";
                return RedirectToAction("Index");
            }

            return View("~/Views/MyPet/Delete.cshtml", thuCung);
        }

        // POST: MyPet/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("MyPet/Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int accountId))
            {
                return RedirectToAction("Index", "Login");
            }

            // Tìm khách hàng liên kết với tài khoản
            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(k => k.TaiKhoanId == accountId);

            if (khachHang == null)
            {
                return NotFound();
            }

            // Kiểm tra thú cưng thuộc về khách hàng này
            var thuCung = await _context.ThuCungs
                .FirstOrDefaultAsync(t => t.Id == id && t.KhachHangId == khachHang.Id);

            if (thuCung == null)
            {
                TempData["Error"] = "Không tìm thấy thú cưng hoặc bạn không có quyền xóa!";
                return RedirectToAction("Index");
            }

            try
            {
                // Kiểm tra xem thú cưng có đang trong đặt lịch nào không
                var hasDatLich = await _context.DatLiches
                    .AnyAsync(d => d.ThuCungId == id && d.TrangThai != "Hoàn thành" && d.TrangThai != "Hủy");

                if (hasDatLich)
                {
                    TempData["Error"] = "Không thể xóa thú cưng đang có lịch hẹn chưa hoàn thành!";
                    return RedirectToAction("Index");
                }

                _context.ThuCungs.Remove(thuCung);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Xóa thú cưng thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa thú cưng");
                TempData["Error"] = "Đã xảy ra lỗi khi xóa thú cưng!";
                return RedirectToAction("Index");
            }
        }
    }
}

