using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spatc.Models;

namespace spatc.Controllers.Admin
{
    [Authorize]
    [Route("Admin/[controller]")]
    public class NhanVienController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NhanVienController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/NhanVien
        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách nhân viên từ bảng NhanVien
            var nhanViens = await _context.NhanViens.OrderByDescending(n => n.NgayTao).ToListAsync();
            
            // Nếu không có dữ liệu, có thể hiển thị thông báo hoặc gợi ý tạo mới
            if (!nhanViens.Any())
            {
                ViewBag.Message = "Chưa có nhân viên nào. Bạn có thể thêm nhân viên mới bằng cách click nút 'Thêm nhân viên mới' ở trên.";
            }
            
            return View("~/Views/Admin/NhanVien/Index.cshtml", nhanViens);
        }

        // GET: Admin/NhanVien/Details/5
        [HttpGet]
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens
                .Include(n => n.DatLiches)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (nhanVien == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/NhanVien/Details.cshtml", nhanVien);
        }

        // GET: Admin/NhanVien/Create
        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/NhanVien/Create.cshtml");
        }

        // POST: Admin/NhanVien/Create
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HoTen,ViTri,SoDienThoai,Email,AnhNhanVien,LichLamViec,TrangThai")] NhanVien nhanVien)
        {
            if (ModelState.IsValid)
            {
                nhanVien.NgayTao = DateTime.Now;
                _context.Add(nhanVien);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm nhân viên thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Admin/NhanVien/Create.cshtml", nhanVien);
        }

        // GET: Admin/NhanVien/Edit/5
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens.FindAsync(id);
            if (nhanVien == null)
            {
                return NotFound();
            }
            return View("~/Views/Admin/NhanVien/Edit.cshtml", nhanVien);
        }

        // POST: Admin/NhanVien/Edit/5
        [HttpPost]
        [Route("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,HoTen,ViTri,SoDienThoai,Email,AnhNhanVien,LichLamViec,TrangThai,NgayTao")] NhanVien nhanVien)
        {
            if (id != nhanVien.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nhanVien);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật nhân viên thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhanVienExists(nhanVien.Id))
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
            return View("~/Views/Admin/NhanVien/Edit.cshtml", nhanVien);
        }

        // GET: Admin/NhanVien/Delete/5
        [HttpGet]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens.FirstOrDefaultAsync(m => m.Id == id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/NhanVien/Delete.cshtml", nhanVien);
        }

        // POST: Admin/NhanVien/Delete/5
        [HttpPost]
        [Route("Delete/{id}")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nhanVien = await _context.NhanViens.FindAsync(id);
            if (nhanVien != null)
            {
                _context.NhanViens.Remove(nhanVien);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa nhân viên thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool NhanVienExists(int id)
        {
            return _context.NhanViens.Any(e => e.Id == id);
        }
    }
}

