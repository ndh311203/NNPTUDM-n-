using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spatc.Models;

namespace spatc.Controllers.Admin
{
    [Authorize]
    [Route("Admin/[controller]")]
    public class FoodProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FoodProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/FoodProduct
        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View("~/Views/Admin/FoodProduct/Index.cshtml", await _context.FoodProducts.OrderByDescending(f => f.CreatedAt).ToListAsync());
        }

        // GET: Admin/FoodProduct/Details/5
        [HttpGet]
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var foodProduct = await _context.FoodProducts.FirstOrDefaultAsync(m => m.Id == id);
            if (foodProduct == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/FoodProduct/Details.cshtml", foodProduct);
        }

        // GET: Admin/FoodProduct/Create
        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/FoodProduct/Create.cshtml");
        }

        // POST: Admin/FoodProduct/Create
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Brand,Type,Weight,Price,Description,Image,SoLuongTon,TrangThai")] FoodProduct foodProduct)
        {
            if (ModelState.IsValid)
            {
                foodProduct.CreatedAt = DateTime.Now;
                foodProduct.UpdatedAt = DateTime.Now;
                foodProduct.TrangThai = true; // Đảm bảo mặc định hiển thị ở cửa hàng
                foodProduct.IsDeleted = false; // Đảm bảo không bị soft delete
                _context.Add(foodProduct);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm thức ăn thành công! Sản phẩm sẽ hiển thị ở cửa hàng.";
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Admin/FoodProduct/Create.cshtml", foodProduct);
        }

        // GET: Admin/FoodProduct/Edit/5
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var foodProduct = await _context.FoodProducts.FindAsync(id);
            if (foodProduct == null)
            {
                return NotFound();
            }
            return View("~/Views/Admin/FoodProduct/Edit.cshtml", foodProduct);
        }

        // POST: Admin/FoodProduct/Edit/5
        [HttpPost]
        [Route("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Brand,Type,Weight,Price,Description,Image,SoLuongTon,TrangThai,CreatedAt")] FoodProduct foodProduct)
        {
            if (id != foodProduct.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    foodProduct.UpdatedAt = DateTime.Now;
                    _context.Update(foodProduct);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật thức ăn thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FoodProductExists(foodProduct.Id))
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
            return View("~/Views/Admin/FoodProduct/Edit.cshtml", foodProduct);
        }

        // GET: Admin/FoodProduct/Delete/5
        [HttpGet]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var foodProduct = await _context.FoodProducts.FirstOrDefaultAsync(m => m.Id == id);
            if (foodProduct == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/FoodProduct/Delete.cshtml", foodProduct);
        }

        // POST: Admin/FoodProduct/Delete/5
        [HttpPost]
        [Route("Delete/{id}")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var foodProduct = await _context.FoodProducts.FindAsync(id);
            if (foodProduct != null)
            {
                _context.FoodProducts.Remove(foodProduct);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa thức ăn thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool FoodProductExists(int id)
        {
            return _context.FoodProducts.Any(e => e.Id == id);
        }
    }
}

