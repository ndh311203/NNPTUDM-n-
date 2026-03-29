using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spatc.Models;

namespace spatc.Controllers.Admin
{
    [Authorize(Roles = "Admin,NhanVien")]
    [Route("Admin/[controller]")]
    public class KhachSanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KhachSanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/KhachSan
        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            // Kiểm tra quyền
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (userRole != "Admin" && userRole != "NhanVien")
            {
                TempData["Error"] = "Bạn không có quyền truy cập trang này!";
                return RedirectToAction("Index", "Home");
            }

            // Lấy chỉ các dịch vụ có LoaiDichVu = "KhachSan" từ bảng DichVus
            return View("~/Views/Admin/KhachSan/Index.cshtml", 
                await _context.DichVus
                    .Where(d => d.LoaiDichVu == "KhachSan")
                    .OrderByDescending(d => d.NgayTao)
                    .ToListAsync());
        }

        // GET: Admin/KhachSan/Details/5
        [HttpGet]
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dichVu = await _context.DichVus
                .Where(d => d.Id == id && d.LoaiDichVu == "KhachSan")
                .FirstOrDefaultAsync();
            
            if (dichVu == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/KhachSan/Details.cshtml", dichVu);
        }

        // GET: Admin/KhachSan/Create
        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/KhachSan/Create.cshtml");
        }

        // POST: Admin/KhachSan/Create
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenDichVu,MoTa,Gia,HinhAnh,TrangThai,SucChua,LoaiPhong,BaoGomBuaAn,DichVuBoSung,ThoiGianNhanTra")] DichVu dichVu)
        {
            if (ModelState.IsValid)
            {
                dichVu.NgayTao = DateTime.Now;
                dichVu.LoaiDichVu = "KhachSan"; // Tự động set loại dịch vụ là KhachSan
                dichVu.TrangThai = true; // Đảm bảo mặc định hiển thị ở trang công khai
                _context.Add(dichVu);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm dịch vụ khách sạn thành công! Dịch vụ sẽ hiển thị ở trang công khai.";
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Admin/KhachSan/Create.cshtml", dichVu);
        }

        // GET: Admin/KhachSan/Edit/5
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dichVu = await _context.DichVus
                .Where(d => d.Id == id && d.LoaiDichVu == "KhachSan")
                .FirstOrDefaultAsync();
            if (dichVu == null)
            {
                return NotFound();
            }
            return View("~/Views/Admin/KhachSan/Edit.cshtml", dichVu);
        }

        // POST: Admin/KhachSan/Edit/5
        [HttpPost]
        [Route("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TenDichVu,MoTa,Gia,HinhAnh,TrangThai,NgayTao,SucChua,LoaiPhong,BaoGomBuaAn,DichVuBoSung,ThoiGianNhanTra")] DichVu dichVu)
        {
            if (id != dichVu.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    dichVu.LoaiDichVu = "KhachSan"; // Đảm bảo luôn là KhachSan
                    _context.Update(dichVu);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật dịch vụ khách sạn thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DichVuExists(dichVu.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Admin/KhachSan/Edit.cshtml", dichVu);
        }

        // GET: Admin/KhachSan/Delete/5
        [HttpGet]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dichVu = await _context.DichVus
                .Where(d => d.Id == id && d.LoaiDichVu == "KhachSan")
                .FirstOrDefaultAsync();
            
            if (dichVu == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/KhachSan/Delete.cshtml", dichVu);
        }

        // POST: Admin/KhachSan/Delete/5
        [HttpPost]
        [Route("Delete/{id}")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dichVu = await _context.DichVus
                .Where(d => d.Id == id && d.LoaiDichVu == "KhachSan")
                .FirstOrDefaultAsync();

            if (dichVu == null)
            {
                return NotFound();
            }

            _context.DichVus.Remove(dichVu);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Xóa dịch vụ khách sạn thành công!";

            return RedirectToAction(nameof(Index));
        }

        private bool DichVuExists(int id)
        {
            return _context.DichVus.Any(e => e.Id == id && e.LoaiDichVu == "KhachSan");
        }
    }
}

