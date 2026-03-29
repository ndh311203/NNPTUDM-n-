using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spatc.Models;

namespace spatc.Controllers.Admin
{
    [Authorize(Roles = "Admin,NhanVien")]
    [Route("Admin/[controller]")]
    public class GroomingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GroomingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Grooming
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

            // Lấy chỉ các dịch vụ có LoaiDichVu = "Grooming" từ bảng DichVus
            return View("~/Views/Admin/Grooming/Index.cshtml", 
                await _context.DichVus
                    .Where(d => d.LoaiDichVu == "Grooming")
                    .OrderByDescending(d => d.NgayTao)
                    .ToListAsync());
        }

        // GET: Admin/Grooming/Details/5
        [HttpGet]
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dichVu = await _context.DichVus
                .Where(d => d.Id == id && d.LoaiDichVu == "Grooming")
                .FirstOrDefaultAsync();
            
            if (dichVu == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/Grooming/Details.cshtml", dichVu);
        }

        // GET: Admin/Grooming/Create
        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/Grooming/Create.cshtml");
        }

        // POST: Admin/Grooming/Create
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenDichVu,MoTa,Gia,HinhAnh,TrangThai,ThoiGianThucHien,KichThuocThuCung,LoaiDichVuGrooming")] DichVu dichVu)
        {
            if (ModelState.IsValid)
            {
                dichVu.NgayTao = DateTime.Now;
                dichVu.LoaiDichVu = "Grooming"; // Tự động set loại dịch vụ là Grooming
                dichVu.TrangThai = true; // Đảm bảo mặc định hiển thị ở trang công khai
                _context.Add(dichVu);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm dịch vụ Grooming thành công! Dịch vụ sẽ hiển thị ở trang công khai.";
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Admin/Grooming/Create.cshtml", dichVu);
        }

        // GET: Admin/Grooming/Edit/5
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dichVu = await _context.DichVus
                .Where(d => d.Id == id && d.LoaiDichVu == "Grooming")
                .FirstOrDefaultAsync();
            if (dichVu == null)
            {
                return NotFound();
            }
            return View("~/Views/Admin/Grooming/Edit.cshtml", dichVu);
        }

        // POST: Admin/Grooming/Edit/5
        [HttpPost]
        [Route("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TenDichVu,MoTa,Gia,HinhAnh,TrangThai,NgayTao,ThoiGianThucHien,KichThuocThuCung,LoaiDichVuGrooming")] DichVu dichVu)
        {
            if (id != dichVu.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    dichVu.LoaiDichVu = "Grooming"; // Đảm bảo luôn là Grooming
                    _context.Update(dichVu);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật dịch vụ Grooming thành công!";
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
            return View("~/Views/Admin/Grooming/Edit.cshtml", dichVu);
        }

        // GET: Admin/Grooming/Delete/5
        [HttpGet]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dichVu = await _context.DichVus
                .Where(d => d.Id == id && d.LoaiDichVu == "Grooming")
                .FirstOrDefaultAsync();
            
            if (dichVu == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/Grooming/Delete.cshtml", dichVu);
        }

        // POST: Admin/Grooming/Delete/5
        [HttpPost]
        [Route("Delete/{id}")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dichVu = await _context.DichVus
                .Where(d => d.Id == id && d.LoaiDichVu == "Grooming")
                .FirstOrDefaultAsync();

            if (dichVu == null)
            {
                return NotFound();
            }

            _context.DichVus.Remove(dichVu);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Xóa dịch vụ Grooming thành công!";

            return RedirectToAction(nameof(Index));
        }

        private bool DichVuExists(int id)
        {
            return _context.DichVus.Any(e => e.Id == id && e.LoaiDichVu == "Grooming");
        }
    }
}

