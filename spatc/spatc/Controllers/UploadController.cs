using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace spatc.Controllers
{
    /// <summary>
    /// Controller xử lý upload file
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<UploadController> _logger;

        public UploadController(IWebHostEnvironment environment, ILogger<UploadController> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        /// <summary>
        /// POST /api/upload/image
        /// Upload ảnh cho thông báo tình trạng sức khỏe
        /// </summary>
        [HttpPost("image")]
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {
                _logger.LogInformation("=== Upload Image Request ===");
                _logger.LogInformation("File received: {FileName}, Size: {FileSize} bytes", 
                    file?.FileName ?? "null", file?.Length ?? 0);

                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("File is null or empty");
                    return BadRequest(new { success = false, message = "Không có file được chọn" });
                }

                // Kiểm tra định dạng file
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { success = false, message = "Định dạng file không hợp lệ. Chỉ chấp nhận: JPG, JPEG, PNG, GIF, WEBP" });
                }

                // Kiểm tra kích thước file (tối đa 5MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new { success = false, message = "File quá lớn. Kích thước tối đa: 5MB" });
                }

                // Tạo thư mục lưu ảnh
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "health-status");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Tạo tên file unique
                var fileName = $"health_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid():N}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Lưu file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Trả về URL
                var fileUrl = $"/uploads/health-status/{fileName}";

                _logger.LogInformation("✅ Upload thành công: {FileName}, URL: {FileUrl}", fileName, fileUrl);
                return Ok(new { success = true, url = fileUrl, fileName = fileName });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi upload ảnh");
                return StatusCode(500, new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }
    }
}

