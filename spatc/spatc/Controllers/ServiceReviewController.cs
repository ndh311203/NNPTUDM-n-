using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spatc.Models;

namespace spatc.Controllers
{
    [Route("ServiceReview")]
    public class ServiceReviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServiceReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: ServiceReview/Create
        [HttpPost]
        [Route("Create")]
        [IgnoreAntiforgeryToken] // Cho phép gửi đánh giá công khai
        public async Task<IActionResult> Create([FromBody] CreateServiceReviewRequest request)
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

                if (request.DichVuId <= 0)
                {
                    return Json(new { success = false, message = "Mã dịch vụ không hợp lệ" });
                }

                // Kiểm tra dịch vụ có tồn tại không
                var dichVu = await _context.DichVus.FindAsync(request.DichVuId);
                if (dichVu == null)
                {
                    return Json(new { success = false, message = "Dịch vụ không tồn tại" });
                }

                var review = new ServiceReview
                {
                    DichVuId = request.DichVuId,
                    Rating = request.Rating,
                    Comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim(),
                    Images = string.IsNullOrWhiteSpace(request.Images) ? null : request.Images.Trim(),
                    CustomerName = request.CustomerName.Trim(),
                    CustomerEmail = string.IsNullOrWhiteSpace(request.CustomerEmail) ? null : request.CustomerEmail.Trim(),
                    Status = "approved", // Tự động approve, admin có thể xóa sau
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.ServiceReviews.Add(review);
                await _context.SaveChangesAsync();

                // Cập nhật rating trung bình và số lượt đánh giá
                await UpdateServiceRating(request.DichVuId);

                return Json(new { success = true, message = "Đánh giá thành công!", reviewId = review.Id });
            }
            catch (DbUpdateException dbEx)
            {
                return Json(new { success = false, message = $"Lỗi database: {dbEx.Message}" });
            }
            catch (Exception ex)
            {
                // Log lỗi để debug
                System.Diagnostics.Debug.WriteLine($"Service Review Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                
                return Json(new { 
                    success = false, 
                    message = $"Lỗi: {ex.Message}",
                    error = ex.GetType().Name
                });
            }
        }

        // GET: ServiceReview/GetReviews
        [HttpGet]
        [Route("GetReviews")]
        public async Task<IActionResult> GetReviews(int dichVuId, int page = 1, int pageSize = 10)
        {
            try
            {
                var reviews = await _context.ServiceReviews
                    .Where(r => r.DichVuId == dichVuId && 
                               r.Status == "approved")
                    .OrderByDescending(r => r.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var totalReviews = await _context.ServiceReviews
                    .CountAsync(r => r.DichVuId == dichVuId && 
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

        // GET: ServiceReview/GetRatingStats
        [HttpGet]
        [Route("GetRatingStats")]
        public async Task<IActionResult> GetRatingStats(int dichVuId)
        {
            try
            {
                var reviews = await _context.ServiceReviews
                    .Where(r => r.DichVuId == dichVuId && 
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

        private async Task UpdateServiceRating(int dichVuId)
        {
            var reviews = await _context.ServiceReviews
                .Where(r => r.DichVuId == dichVuId && 
                           r.Status == "approved")
                .ToListAsync();

            var averageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;
            var reviewCount = reviews.Count;

            var dichVu = await _context.DichVus.FindAsync(dichVuId);
            if (dichVu != null)
            {
                dichVu.AverageRating = (decimal)averageRating;
                dichVu.ReviewCount = reviewCount;
            }

            await _context.SaveChangesAsync();
        }
    }

    public class CreateServiceReviewRequest
    {
        public int DichVuId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string? Images { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? CustomerEmail { get; set; }
    }
}











