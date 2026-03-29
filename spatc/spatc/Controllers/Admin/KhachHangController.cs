using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spatc.Models;

namespace spatc.Controllers.Admin
{
    [Authorize]
    [Route("Admin/[controller]")]
    public class KhachHangController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KhachHangController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/KhachHang
        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View("~/Views/Admin/KhachHang/Index.cshtml", await _context.KhachHangs
                .Include(k => k.ThuCungs)
                .OrderByDescending(k => k.NgayTao)
                .ToListAsync());
        }

        // GET: Admin/KhachHang/Details/5
        [HttpGet]
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs
                .Include(k => k.ThuCungs)
                .ThenInclude(t => t.DatLiches)
                .ThenInclude(d => d.DichVu)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (khachHang == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/KhachHang/Details.cshtml", khachHang);
        }

        // GET: Admin/KhachHang/Edit/5
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs.FindAsync(id);
            if (khachHang == null)
            {
                return NotFound();
            }
            return View("~/Views/Admin/KhachHang/Edit.cshtml", khachHang);
        }

        // POST: Admin/KhachHang/Edit/5
        [HttpPost]
        [Route("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,HoTen,SoDienThoai,Email,DiaChi,GhiChu")] KhachHang khachHang)
        {
            if (id != khachHang.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.KhachHangs.FindAsync(id);
                    if (existing != null)
                    {
                        existing.HoTen = khachHang.HoTen;
                        existing.Email = khachHang.Email;
                        existing.DiaChi = khachHang.DiaChi;
                        existing.GhiChu = khachHang.GhiChu;
                        _context.Update(existing);
                        await _context.SaveChangesAsync();
                        TempData["Success"] = "Cập nhật thông tin khách hàng thành công!";
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhachHangExists(khachHang.Id))
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
            return View("~/Views/Admin/KhachHang/Edit.cshtml", khachHang);
        }

        private bool KhachHangExists(int id)
        {
            return _context.KhachHangs.Any(e => e.Id == id);
        }
    }
}

