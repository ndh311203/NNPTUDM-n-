using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spatc.Models;

namespace spatc.Controllers
{
    [Route("ProductReview")]
    public class ProductReviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: ProductReview/Create
        [HttpPost]
        [Route("Create")]
        [IgnoreAntiforgeryToken] // Cho phép gửi đánh giá công khai
        public async Task<IActionResult> Create([FromBody] CreateReviewRequest request)
        {
            try
            {
                // Validation
                if (request == null)
                {
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
                }

                if (string.IsNullOrWhiteSpace(request.CustomerName))
                {
                    return Json(new { success = false, message = "Vui lòng nhập tên của bạn" });
                }

                if (request.Rating < 1 || request.Rating > 5)
                {
                    return Json(new { success = false, message = "Đánh giá phải từ 1 đến 5 sao" });
                }

                if (request.ProductId <= 0)
                {
                    return Json(new { success = false, message = "Mã sản phẩm không hợp lệ" });
                }

                if (string.IsNullOrWhiteSpace(request.ProductType))
                {
                    return Json(new { success = false, message = "Loại sản phẩm không hợp lệ" });
                }

                var review = new ProductReview
                {
                    ProductId = request.ProductId,
                    ProductType = request.ProductType.Trim(),
                    Rating = request.Rating,
                    Comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim(),
                    Images = string.IsNullOrWhiteSpace(request.Images) ? null : request.Images.Trim(),
                    CustomerName = request.CustomerName.Trim(),
                    CustomerEmail = string.IsNullOrWhiteSpace(request.CustomerEmail) ? null : request.CustomerEmail.Trim(),
                    Status = "approved", // Tự động approve, admin có thể xóa sau
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.ProductReviews.Add(review);
                await _context.SaveChangesAsync();

                // Cập nhật rating trung bình và số lượt đánh giá
                await UpdateProductRating(request.ProductId, request.ProductType);

                return Json(new { success = true, message = "Đánh giá thành công!", reviewId = review.Id });
            }
            catch (DbUpdateException dbEx)
            {
                return Json(new { success = false, message = $"Lỗi database: {dbEx.Message}" });
            }
            catch (Exception ex)
            {
                // Log lỗi để debug
                System.Diagnostics.Debug.WriteLine($"Review Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                
                return Json(new { 
                    success = false, 
                    message = $"Lỗi: {ex.Message}",
                    error = ex.GetType().Name
                });
            }
        }

        // GET: ProductReview/GetReviews
        [HttpGet]
        [Route("GetReviews")]
        public async Task<IActionResult> GetReviews(int productId, string productType, int page = 1, int pageSize = 10)
        {
            try
            {
                var reviews = await _context.ProductReviews
                    .Where(r => r.ProductId == productId && 
                               r.ProductType == productType && 
                               r.Status == "approved")
                    .OrderByDescending(r => r.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var totalReviews = await _context.ProductReviews
                    .CountAsync(r => r.ProductId == productId && 
                                   r.ProductType == productType && 
                                   r.Status == "approved");

                return Json(new
                {
                    success = true,
                    reviews = reviews.Select(r => new
                    {
                        id = r.Id,
                        rating = r.Rating,
                        comment = r.Comment,
                        images = string.IsNullOrEmpty(r.Images) ? new string[0] : r.Images.Split(',', StringSplitOptions.RemoveEmptyEntries),
                        customerName = r.CustomerName,
                        createdAt = r.CreatedAt.ToString("dd/MM/yyyy HH:mm")
                    }),
                    totalReviews = totalReviews,
                    currentPage = page,
                    totalPages = (int)Math.Ceiling(totalReviews / (double)pageSize)
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: ProductReview/GetRatingStats
        [HttpGet]
        [Route("GetRatingStats")]
        public async Task<IActionResult> GetRatingStats(int productId, string productType)
        {
            try
            {
                var reviews = await _context.ProductReviews
                    .Where(r => r.ProductId == productId && 
                               r.ProductType == productType && 
                               r.Status == "approved")
                    .ToListAsync();

                if (!reviews.Any())
                {
                    return Json(new
                    {
                        success = true,
                        averageRating = 0,
                        totalReviews = 0,
                        ratingDistribution = new { r5 = 0, r4 = 0, r3 = 0, r2 = 0, r1 = 0 }
                    });
                }

                var averageRating = reviews.Average(r => r.Rating);
                var ratingDistribution = new
                {
                    r5 = reviews.Count(r => r.Rating == 5),
                    r4 = reviews.Count(r => r.Rating == 4),
                    r3 = reviews.Count(r => r.Rating == 3),
                    r2 = reviews.Count(r => r.Rating == 2),
                    r1 = reviews.Count(r => r.Rating == 1)
                };

                return Json(new
                {
                    success = true,
                    averageRating = Math.Round(averageRating, 1),
                    totalReviews = reviews.Count,
                    ratingDistribution = ratingDistribution
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private async Task UpdateProductRating(int productId, string productType)
        {
            var reviews = await _context.ProductReviews
                .Where(r => r.ProductId == productId && 
                           r.ProductType == productType && 
                           r.Status == "approved")
                .ToListAsync();

            var averageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;
            var reviewCount = reviews.Count;

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

    public class CreateReviewRequest
    {
        public int ProductId { get; set; }
        public string ProductType { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string? Images { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? CustomerEmail { get; set; }
    }
}

