using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using spatc.Models;

namespace spatc.Controllers.Admin
{
    [Authorize]
    [Route("Admin/[controller]")]
    public class ThuCungController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ThuCungController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/ThuCung
        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var thuCungs = await _context.ThuCungs
                .Include(t => t.KhachHang)
                .OrderByDescending(t => t.NgayTao)
                .ToListAsync();
            return View("~/Views/Admin/ThuCung/Index.cshtml", thuCungs);
        }

        // GET: Admin/ThuCung/Details/5
        [HttpGet]
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thuCung = await _context.ThuCungs
                .Include(t => t.KhachHang)
                .Include(t => t.DatLiches)
                .ThenInclude(d => d.DichVu)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (thuCung == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/ThuCung/Details.cshtml", thuCung);
        }

        // GET: Admin/ThuCung/Create
        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/ThuCung/Create.cshtml");
        }

        // POST: Admin/ThuCung/Create
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenThuCung,Loai,Giong,Tuoi,CanNang,GhiChuSucKhoe,HinhAnh")] ThuCung thuCung)
        {
            // Bỏ qua validation cho KhachHangId
            ModelState.Remove("KhachHangId");
            
            if (ModelState.IsValid)
            {
                // Tự động gán khách hàng đầu tiên hoặc tạo khách hàng mặc định
                var firstKhachHang = await _context.KhachHangs.FirstOrDefaultAsync();
                if (firstKhachHang == null)
                {
                    // Tạo khách hàng mặc định nếu chưa có
                    firstKhachHang = new KhachHang
                    {
                        HoTen = "Khách hàng mặc định",
                        SoDienThoai = "0000000000",
                        NgayTao = DateTime.Now
                    };
                    _context.KhachHangs.Add(firstKhachHang);
                    await _context.SaveChangesAsync();
                }
                
                thuCung.KhachHangId = firstKhachHang.Id;
                thuCung.NgayTao = DateTime.Now;
                _context.Add(thuCung);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm thú cưng thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Admin/ThuCung/Create.cshtml", thuCung);
        }

        // GET: Admin/ThuCung/Edit/5
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thuCung = await _context.ThuCungs.FindAsync(id);
            if (thuCung == null)
            {
                return NotFound();
            }
            ViewData["KhachHangId"] = new SelectList(_context.KhachHangs, "Id", "HoTen", thuCung.KhachHangId);
            return View("~/Views/Admin/ThuCung/Edit.cshtml", thuCung);
        }

        // POST: Admin/ThuCung/Edit/5
        [HttpPost]
        [Route("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TenThuCung,Loai,Giong,Tuoi,CanNang,GhiChuSucKhoe,HinhAnh,KhachHangId,NgayTao")] ThuCung thuCung)
        {
            if (id != thuCung.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(thuCung);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật thú cưng thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThuCungExists(thuCung.Id))
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
            ViewData["KhachHangId"] = new SelectList(_context.KhachHangs, "Id", "HoTen", thuCung.KhachHangId);
            return View("~/Views/Admin/ThuCung/Edit.cshtml", thuCung);
        }

        // GET: Admin/ThuCung/Delete/5
        [HttpGet]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thuCung = await _context.ThuCungs
                .Include(t => t.KhachHang)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (thuCung == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/ThuCung/Delete.cshtml", thuCung);
        }

        // POST: Admin/ThuCung/Delete/5
        [HttpPost]
        [Route("Delete/{id}")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var thuCung = await _context.ThuCungs.FindAsync(id);
            if (thuCung != null)
            {
                _context.ThuCungs.Remove(thuCung);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa thú cưng thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ThuCungExists(int id)
        {
            return _context.ThuCungs.Any(e => e.Id == id);
        }
    }
}

