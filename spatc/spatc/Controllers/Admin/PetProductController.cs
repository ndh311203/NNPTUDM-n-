using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spatc.Models;

namespace spatc.Controllers.Admin
{
    [Authorize]
    [Route("Admin/[controller]")]
    public class PetProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PetProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/PetProduct
        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View("~/Views/Admin/PetProduct/Index.cshtml", await _context.PetProducts.OrderByDescending(p => p.CreatedAt).ToListAsync());
        }

        // GET: Admin/PetProduct/Details/5
        [HttpGet]
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var petProduct = await _context.PetProducts.FirstOrDefaultAsync(m => m.Id == id);
            if (petProduct == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/PetProduct/Details.cshtml", petProduct);
        }

        // GET: Admin/PetProduct/Create
        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/PetProduct/Create.cshtml");
        }

        // POST: Admin/PetProduct/Create
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Species,Age,Gender,HealthStatus,Price,Description,Images,TrangThai")] PetProduct petProduct)
        {
            if (ModelState.IsValid)
            {
                petProduct.CreatedAt = DateTime.Now;
                petProduct.UpdatedAt = DateTime.Now;
                petProduct.TrangThai = true; // Đảm bảo mặc định hiển thị ở cửa hàng
                petProduct.IsDeleted = false; // Đảm bảo không bị soft delete
                _context.Add(petProduct);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm thú cưng thành công! Sản phẩm sẽ hiển thị ở cửa hàng.";
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Admin/PetProduct/Create.cshtml", petProduct);
        }

        // GET: Admin/PetProduct/Edit/5
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var petProduct = await _context.PetProducts.FindAsync(id);
            if (petProduct == null)
            {
                return NotFound();
            }
            return View("~/Views/Admin/PetProduct/Edit.cshtml", petProduct);
        }

        // POST: Admin/PetProduct/Edit/5
        [HttpPost]
        [Route("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Species,Age,Gender,HealthStatus,Price,Description,Images,TrangThai,CreatedAt")] PetProduct petProduct)
        {
            if (id != petProduct.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    petProduct.UpdatedAt = DateTime.Now;
                    _context.Update(petProduct);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật thú cưng thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PetProductExists(petProduct.Id))
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
            return View("~/Views/Admin/PetProduct/Edit.cshtml", petProduct);
        }

        // GET: Admin/PetProduct/Delete/5
        [HttpGet]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var petProduct = await _context.PetProducts.FirstOrDefaultAsync(m => m.Id == id);
            if (petProduct == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/PetProduct/Delete.cshtml", petProduct);
        }

        // POST: Admin/PetProduct/Delete/5
        [HttpPost]
        [Route("Delete/{id}")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var petProduct = await _context.PetProducts.FindAsync(id);
            if (petProduct != null)
            {
                _context.PetProducts.Remove(petProduct);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa thú cưng thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PetProductExists(int id)
        {
            return _context.PetProducts.Any(e => e.Id == id);
        }
    }
}

