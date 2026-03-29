using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spatc.Models;

namespace spatc.Controllers.Admin
{
    [Authorize(Roles = "Admin,NhanVien")]
    [Route("Admin/[controller]")]
    public class CongViecController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CongViecController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/CongViec
        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View("~/Views/Admin/CongViec/Index.cshtml", 
                await _context.CongViecs.OrderBy(c => c.TenCongViec).ToListAsync());
        }

        // GET: Admin/CongViec/Create
        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/CongViec/Create.cshtml");
        }

        // POST: Admin/CongViec/Create
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenCongViec,MoTa")] CongViec congViec)
        {
            if (ModelState.IsValid)
            {
                congViec.NgayTao = DateTime.Now;
                _context.Add(congViec);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm công việc thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Admin/CongViec/Create.cshtml", congViec);
        }

        // GET: Admin/CongViec/Edit/5
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var congViec = await _context.CongViecs.FindAsync(id);
            if (congViec == null)
            {
                return NotFound();
            }
            return View("~/Views/Admin/CongViec/Edit.cshtml", congViec);
        }

        // POST: Admin/CongViec/Edit/5
        [HttpPost]
        [Route("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TenCongViec,MoTa,NgayTao")] CongViec congViec)
        {
            if (id != congViec.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(congViec);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật công việc thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CongViecExists(congViec.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View("~/Views/Admin/CongViec/Edit.cshtml", congViec);
        }

        // POST: Admin/CongViec/Delete/5
        [HttpPost]
        [Route("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var congViec = await _context.CongViecs.FindAsync(id);
            if (congViec != null)
            {
                _context.CongViecs.Remove(congViec);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa công việc thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/CongViec/SeedData - Thêm dữ liệu mẫu
        [HttpPost]
        [Route("SeedData")]
        public async Task<IActionResult> SeedData()
        {
            if (await _context.CongViecs.AnyAsync())
            {
                return Json(new { success = false, message = "Đã có dữ liệu công việc!" });
            }

            var congViecs = new List<CongViec>
            {
                new CongViec { TenCongViec = "Tắm thú cưng", MoTa = "Tắm rửa và vệ sinh cho thú cưng", NgayTao = DateTime.Now },
                new CongViec { TenCongViec = "Cắt tỉa lông", MoTa = "Cắt tỉa và tạo kiểu lông cho thú cưng", NgayTao = DateTime.Now },
                new CongViec { TenCongViec = "Dọn vệ sinh chuồng", MoTa = "Vệ sinh và dọn dẹp chuồng trại", NgayTao = DateTime.Now },
                new CongViec { TenCongViec = "Chăm sóc khách sạn", MoTa = "Chăm sóc thú cưng tại khách sạn", NgayTao = DateTime.Now },
                new CongViec { TenCongViec = "Quản lý hàng hóa", MoTa = "Quản lý và kiểm kê hàng hóa trong cửa hàng", NgayTao = DateTime.Now },
                new CongViec { TenCongViec = "Phụ bán hàng", MoTa = "Hỗ trợ bán hàng và tư vấn khách hàng", NgayTao = DateTime.Now }
            };

            _context.CongViecs.AddRange(congViecs);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = $"Đã thêm {congViecs.Count} công việc mẫu!" });
        }

        private bool CongViecExists(int id)
        {
            return _context.CongViecs.Any(e => e.Id == id);
        }
    }
}

