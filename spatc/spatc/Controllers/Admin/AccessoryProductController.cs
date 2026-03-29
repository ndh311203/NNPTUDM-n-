using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spatc.Models;

namespace spatc.Controllers.Admin
{
    [Authorize]
    [Route("Admin/[controller]")]
    public class AccessoryProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccessoryProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/AccessoryProduct
        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View("~/Views/Admin/AccessoryProduct/Index.cshtml", await _context.AccessoryProducts.OrderByDescending(a => a.CreatedAt).ToListAsync());
        }

        // GET: Admin/AccessoryProduct/Details/5
        [HttpGet]
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accessoryProduct = await _context.AccessoryProducts.FirstOrDefaultAsync(m => m.Id == id);
            if (accessoryProduct == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/AccessoryProduct/Details.cshtml", accessoryProduct);
        }

        // GET: Admin/AccessoryProduct/Create
        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/AccessoryProduct/Create.cshtml");
        }

        // POST: Admin/AccessoryProduct/Create
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Category,Material,Price,Description,Image,SoLuongTon,TrangThai")] AccessoryProduct accessoryProduct)
        {
            if (ModelState.IsValid)
            {
                accessoryProduct.CreatedAt = DateTime.Now;
                accessoryProduct.UpdatedAt = DateTime.Now;
                accessoryProduct.TrangThai = true; // Đảm bảo mặc định hiển thị ở cửa hàng
                accessoryProduct.IsDeleted = false; // Đảm bảo không bị soft delete
                _context.Add(accessoryProduct);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm phụ kiện thành công! Sản phẩm sẽ hiển thị ở cửa hàng.";
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Admin/AccessoryProduct/Create.cshtml", accessoryProduct);
        }

        // GET: Admin/AccessoryProduct/Edit/5
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accessoryProduct = await _context.AccessoryProducts.FindAsync(id);
            if (accessoryProduct == null)
            {
                return NotFound();
            }
            return View("~/Views/Admin/AccessoryProduct/Edit.cshtml", accessoryProduct);
        }

        // POST: Admin/AccessoryProduct/Edit/5
        [HttpPost]
        [Route("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Category,Material,Price,Description,Image,SoLuongTon,TrangThai,CreatedAt")] AccessoryProduct accessoryProduct)
        {
            if (id != accessoryProduct.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    accessoryProduct.UpdatedAt = DateTime.Now;
                    _context.Update(accessoryProduct);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật phụ kiện thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccessoryProductExists(accessoryProduct.Id))
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
            return View("~/Views/Admin/AccessoryProduct/Edit.cshtml", accessoryProduct);
        }

        // GET: Admin/AccessoryProduct/Delete/5
        [HttpGet]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accessoryProduct = await _context.AccessoryProducts.FirstOrDefaultAsync(m => m.Id == id);
            if (accessoryProduct == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/AccessoryProduct/Delete.cshtml", accessoryProduct);
        }

        // POST: Admin/AccessoryProduct/Delete/5
        [HttpPost]
        [Route("Delete/{id}")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var accessoryProduct = await _context.AccessoryProducts.FindAsync(id);
            if (accessoryProduct != null)
            {
                _context.AccessoryProducts.Remove(accessoryProduct);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa phụ kiện thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool AccessoryProductExists(int id)
        {
            return _context.AccessoryProducts.Any(e => e.Id == id);
        }
    }
}

