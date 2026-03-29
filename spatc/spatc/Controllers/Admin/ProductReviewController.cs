using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spatc.Models;

namespace spatc.Controllers.Admin
{
    [Authorize]
    [Route("Admin/[controller]")]
    public class ProductReviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/ProductReview
        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index(string? status, string? productType, int? productId)
        {
            var query = _context.ProductReviews.AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(r => r.Status == status);
            }

            if (!string.IsNullOrEmpty(productType))
            {
                query = query.Where(r => r.ProductType == productType);
            }

            if (productId.HasValue)
            {
                query = query.Where(r => r.ProductId == productId.Value);
            }

            var reviews = await query.OrderByDescending(r => r.CreatedAt).ToListAsync();

            ViewBag.StatusFilter = status;
            ViewBag.ProductTypeFilter = productType;
            ViewBag.ProductIdFilter = productId;
            ViewBag.Statuses = new[] { "approved", "pending", "rejected" };

            return View("~/Views/Admin/ProductReview/Index.cshtml", reviews);
        }

        // GET: Admin/ProductReview/Details/5
        [HttpGet]
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.ProductReviews.FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/ProductReview/Details.cshtml", review);
        }

        // POST: Admin/ProductReview/Delete/5
        [HttpPost]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _context.ProductReviews.FindAsync(id);
            if (review == null)
            {
                return Json(new { success = false, message = "Đánh giá không tồn tại" });
            }

            try
            {
                _context.ProductReviews.Remove(review);
                await _context.SaveChangesAsync();

                // Cập nhật lại rating của sản phẩm
                await UpdateProductRatingAfterDelete(review.ProductId, review.ProductType);

                TempData["Success"] = "Xóa đánh giá thành công!";
                return Json(new { success = true, message = "Xóa thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        // POST: Admin/ProductReview/UpdateStatus
        [HttpPost]
        [Route("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus(int reviewId, string status)
        {
            var review = await _context.ProductReviews.FindAsync(reviewId);
            if (review == null)
            {
                return Json(new { success = false, message = "Đánh giá không tồn tại" });
            }

            var validStatuses = new[] { "approved", "pending", "rejected" };
            if (!validStatuses.Contains(status))
            {
                return Json(new { success = false, message = "Trạng thái không hợp lệ" });
            }

            review.Status = status;
            review.UpdatedAt = DateTime.Now;

            try
            {
                _context.Update(review);
                await _context.SaveChangesAsync();

                // Cập nhật lại rating nếu status thay đổi
                await UpdateProductRatingAfterStatusChange(review.ProductId, review.ProductType);

                TempData["Success"] = "Cập nhật trạng thái thành công!";
                return Json(new { success = true, message = "Cập nhật thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        private async Task UpdateProductRatingAfterDelete(int productId, string productType)
        {
            var reviews = await _context.ProductReviews
                .Where(r => r.ProductId == productId && 
                           r.ProductType == productType && 
                           r.Status == "approved")
                .ToListAsync();

            var averageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;
            var reviewCount = reviews.Count;

            await UpdateProductRating(productId, productType, averageRating, reviewCount);
        }

        private async Task UpdateProductRatingAfterStatusChange(int productId, string productType)
        {
            var reviews = await _context.ProductReviews
                .Where(r => r.ProductId == productId && 
                           r.ProductType == productType && 
                           r.Status == "approved")
                .ToListAsync();

            var averageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;
            var reviewCount = reviews.Count;

            await UpdateProductRating(productId, productType, averageRating, reviewCount);
        }

        private async Task UpdateProductRating(int productId, string productType, double averageRating, int reviewCount)
        {
            switch (productType.ToLower())
            {
                case "pet":
                    var pet = await _context.PetProducts.FindAsync(productId);
                    if (pet != null)
                    {
                        pet.AverageRating = (decimal)averageRating;
                        pet.ReviewCount = reviewCount;
                    }
                    break;

                case "food":
                    var food = await _context.FoodProducts.FindAsync(productId);
                    if (food != null)
                    {
                        food.AverageRating = (decimal)averageRating;
                        food.ReviewCount = reviewCount;
                    }
                    break;

                case "accessory":
                    var accessory = await _context.AccessoryProducts.FindAsync(productId);
                    if (accessory != null)
                    {
                        accessory.AverageRating = (decimal)averageRating;
                        accessory.ReviewCount = reviewCount;
                    }
                    break;
            }

            await _context.SaveChangesAsync();
        }
    }
}

