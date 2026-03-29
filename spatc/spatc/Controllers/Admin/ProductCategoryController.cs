using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using spatc.Models;

namespace spatc.Controllers.Admin
{
    [Authorize(Roles = "Admin,Staff")]
    [Route("Admin/[controller]")]
    public class ProductCategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductCategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/ProductCategory
        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View("~/Views/Admin/ProductCategory/Index.cshtml", await _context.ProductCategories.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name).ToListAsync());
        }

        // GET: Admin/ProductCategory/Details/5
        [HttpGet]
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.ProductCategories.FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/ProductCategory/Details.cshtml", category);
        }

        // GET: Admin/ProductCategory/Create
        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/ProductCategory/Create.cshtml");
        }

        // POST: Admin/ProductCategory/Create
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Slug,Description,DisplayOrder,IsActive")] ProductCategory category)
        {
            // Tự động tạo slug từ name nếu không có
            if (string.IsNullOrWhiteSpace(category.Slug))
            {
                category.Slug = GenerateSlug(category.Name);
            }
            else
            {
                category.Slug = GenerateSlug(category.Slug);
            }

            // Kiểm tra slug đã tồn tại chưa
            if (await _context.ProductCategories.AnyAsync(c => c.Slug == category.Slug))
            {
                ModelState.AddModelError("Slug", "Mã danh mục này đã được sử dụng!");
            }

            if (ModelState.IsValid)
            {
                category.CreatedAt = DateTime.Now;
                category.UpdatedAt = DateTime.Now;
                _context.Add(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm danh mục thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Admin/ProductCategory/Create.cshtml", category);
        }

        // GET: Admin/ProductCategory/Edit/5
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.ProductCategories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View("~/Views/Admin/ProductCategory/Edit.cshtml", category);
        }

        // POST: Admin/ProductCategory/Edit/5
        [HttpPost]
        [Route("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Slug,Description,DisplayOrder,IsActive,CreatedAt")] ProductCategory category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            // Tự động tạo slug từ name nếu không có
            if (string.IsNullOrWhiteSpace(category.Slug))
            {
                category.Slug = GenerateSlug(category.Name);
            }
            else
            {
                category.Slug = GenerateSlug(category.Slug);
            }

            // Kiểm tra slug đã tồn tại chưa (trừ chính nó)
            if (await _context.ProductCategories.AnyAsync(c => c.Slug == category.Slug && c.Id != id))
            {
                ModelState.AddModelError("Slug", "Mã danh mục này đã được sử dụng!");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    category.UpdatedAt = DateTime.Now;
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật danh mục thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            return View("~/Views/Admin/ProductCategory/Edit.cshtml", category);
        }

        // GET: Admin/ProductCategory/Delete/5
        [HttpGet]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.ProductCategories.FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            // Kiểm tra có sản phẩm nào đang dùng danh mục này không
            var productsUsingCategory = await _context.FoodProducts
                .Where(p => p.CategoryId == id)
                .CountAsync();
            productsUsingCategory += await _context.AccessoryProducts
                .Where(p => p.ProductCategoryId == id)
                .CountAsync();

            ViewBag.ProductsUsingCategory = productsUsingCategory;

            return View("~/Views/Admin/ProductCategory/Delete.cshtml", category);
        }

        // POST: Admin/ProductCategory/Delete/5
        [HttpPost]
        [Route("Delete/{id}")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.ProductCategories.FindAsync(id);
            if (category != null)
            {
                // Kiểm tra có sản phẩm nào đang dùng danh mục này không
                var hasProducts = await _context.FoodProducts.AnyAsync(p => p.CategoryId == id) ||
                                 await _context.AccessoryProducts.AnyAsync(p => p.ProductCategoryId == id);

                if (hasProducts)
                {
                    TempData["Error"] = "Không thể xóa danh mục này vì có sản phẩm đang sử dụng!";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                _context.ProductCategories.Remove(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa danh mục thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.ProductCategories.Any(e => e.Id == id);
        }

        private string GenerateSlug(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            // Chuyển thành chữ thường
            text = text.ToLowerInvariant();

            // Loại bỏ dấu tiếng Việt
            text = Regex.Replace(text, @"[àáạảãâầấậẩẫăằắặẳẵ]", "a");
            text = Regex.Replace(text, @"[èéẹẻẽêềếệểễ]", "e");
            text = Regex.Replace(text, @"[ìíịỉĩ]", "i");
            text = Regex.Replace(text, @"[òóọỏõôồốộổỗơờớợởỡ]", "o");
            text = Regex.Replace(text, @"[ùúụủũưừứựửữ]", "u");
            text = Regex.Replace(text, @"[ỳýỵỷỹ]", "y");
            text = Regex.Replace(text, @"[đ]", "d");

            // Thay thế khoảng trắng bằng dấu gạch ngang
            text = Regex.Replace(text, @"\s+", "-");

            // Loại bỏ các ký tự không hợp lệ
            text = Regex.Replace(text, @"[^a-z0-9\-]", "");

            // Loại bỏ nhiều dấu gạch ngang liên tiếp
            text = Regex.Replace(text, @"-+", "-");

            // Loại bỏ dấu gạch ngang ở đầu và cuối
            text = text.Trim('-');

            return text;
        }
    }
}

