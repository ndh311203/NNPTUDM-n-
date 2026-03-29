using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spatc.Models;

namespace spatc.Controllers.Admin
{
    [Authorize]
    [Route("Admin/[controller]")]
    public class VoucherController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VoucherController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Voucher
        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            // Tự động disable voucher hết hạn
            var expiredVouchers = await _context.Vouchers
                .Where(v => v.Status == "active" && v.EndDate < DateTime.Now)
                .ToListAsync();

            foreach (var voucher in expiredVouchers)
            {
                voucher.Status = "inactive";
                voucher.UpdatedAt = DateTime.Now;
            }

            if (expiredVouchers.Any())
            {
                await _context.SaveChangesAsync();
            }

            var vouchers = await _context.Vouchers.OrderByDescending(v => v.CreatedAt).ToListAsync();
            return View("~/Views/Admin/Voucher/Index.cshtml", vouchers);
        }

        // GET: Admin/Voucher/Details/5
        [HttpGet]
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voucher = await _context.Vouchers.FirstOrDefaultAsync(m => m.Id == id);
            if (voucher == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/Voucher/Details.cshtml", voucher);
        }

        // GET: Admin/Voucher/Create
        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/Voucher/Create.cshtml");
        }

        // POST: Admin/Voucher/Create
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Code,Type,Value,MinOrderAmount,MaxDiscount,StartDate,EndDate,Status")] Voucher voucher)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra mã voucher trùng
                var existing = await _context.Vouchers.FirstOrDefaultAsync(v => v.Code == voucher.Code);
                if (existing != null)
                {
                    ModelState.AddModelError("Code", "Mã voucher đã tồn tại");
                    return View("~/Views/Admin/Voucher/Create.cshtml", voucher);
                }

                voucher.CreatedAt = DateTime.Now;
                voucher.UpdatedAt = DateTime.Now;
                _context.Add(voucher);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm voucher thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Admin/Voucher/Create.cshtml", voucher);
        }

        // GET: Admin/Voucher/Edit/5
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher == null)
            {
                return NotFound();
            }
            return View("~/Views/Admin/Voucher/Edit.cshtml", voucher);
        }

        // POST: Admin/Voucher/Edit/5
        [HttpPost]
        [Route("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,Type,Value,MinOrderAmount,MaxDiscount,StartDate,EndDate,Status,CreatedAt")] Voucher voucher)
        {
            if (id != voucher.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    voucher.UpdatedAt = DateTime.Now;
                    _context.Update(voucher);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật voucher thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VoucherExists(voucher.Id))
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
            return View("~/Views/Admin/Voucher/Edit.cshtml", voucher);
        }

        // GET: Admin/Voucher/Delete/5
        [HttpGet]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voucher = await _context.Vouchers.FirstOrDefaultAsync(m => m.Id == id);
            if (voucher == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/Voucher/Delete.cshtml", voucher);
        }

        // POST: Admin/Voucher/Delete/5
        [HttpPost]
        [Route("Delete/{id}")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher != null)
            {
                _context.Vouchers.Remove(voucher);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa voucher thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool VoucherExists(int id)
        {
            return _context.Vouchers.Any(e => e.Id == id);
        }
    }
}

