using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spatc.Models;
using spatc.Services;

namespace spatc.Controllers
{
    /// <summary>
    /// Controller xử lý API thông báo
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService _notificationService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(
            NotificationService notificationService,
            ApplicationDbContext context,
            ILogger<NotificationController> logger)
        {
            _notificationService = notificationService;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// POST /api/notification/create
        /// Tạo thông báo mới
        /// </summary>
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateNotificationRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.LoaiThongBao))
                {
                    return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ" });
                }

                var thongBao = await _notificationService.TaoThongBaoAsync(
                    request.LoaiThongBao,
                    request.TieuDe ?? "",
                    request.NoiDung ?? "",
                    request.NguoiNhanId,
                    request.DatLichId,
                    request.ThuCungId
                );

                return Ok(new { success = true, data = thongBao });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo thông báo");
                return StatusCode(500, new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        /// <summary>
        /// GET /api/notification?userId=123
        /// Lấy danh sách thông báo
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetNotifications([FromQuery] int? userId, [FromQuery] int skip = 0, [FromQuery] int take = 20)
        {
            try
            {
                // Lấy userId từ claim nếu không có trong query
                if (!userId.HasValue)
                {
                    var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    if (int.TryParse(userIdClaim, out int parsedUserId))
                    {
                        userId = parsedUserId;
                    }
                }

                // Kiểm tra nếu là admin (NguoiNhanId = null)
                var isAdmin = User.IsInRole("Admin") || User.IsInRole("NhanVien");
                if (isAdmin)
                {
                    // Admin xem thông báo gửi cho admin (NguoiNhanId = null)
                    // Không cần userId, để null để lấy thông báo cho admin
                    userId = null;
                }

                var thongBaos = await _notificationService.LayDanhSachThongBaoAsync(userId, skip, take);
                var soLuongChuaDoc = await _notificationService.DemThongBaoChuaDocAsync(userId);

                return Ok(new
                {
                    success = true,
                    data = thongBaos,
                    soLuongChuaDoc = soLuongChuaDoc,
                    total = thongBaos.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách thông báo");
                return StatusCode(500, new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        /// <summary>
        /// GET /api/notification/unread-count?userId=123
        /// Lấy số lượng thông báo chưa đọc
        /// </summary>
        [HttpGet("unread-count")]
        [Authorize]
        public async Task<IActionResult> GetUnreadCount([FromQuery] int? userId)
        {
            try
            {
                // Kiểm tra nếu là admin
                var isAdmin = User.IsInRole("Admin") || User.IsInRole("NhanVien");
                if (isAdmin)
                {
                    // Admin xem thông báo gửi cho admin (NguoiNhanId = null)
                    userId = null;
                }
                else
                {
                    // Lấy userId từ claim nếu không có trong query
                    if (!userId.HasValue)
                    {
                        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                        if (int.TryParse(userIdClaim, out int parsedUserId))
                        {
                            userId = parsedUserId;
                        }
                    }
                }

                var count = await _notificationService.DemThongBaoChuaDocAsync(userId);
                return Ok(new { success = true, count = count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đếm thông báo chưa đọc");
                return StatusCode(500, new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        /// <summary>
        /// PUT /api/notification/read
        /// Đánh dấu thông báo đã đọc
        /// </summary>
        [HttpPut("read")]
        [Authorize]
        public async Task<IActionResult> MarkAsRead([FromBody] MarkAsReadRequest request)
        {
            try
            {
                if (request == null || request.Id <= 0)
                {
                    return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ" });
                }

                int? userId = null;
                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (int.TryParse(userIdClaim, out int parsedUserId))
                {
                    userId = parsedUserId;
                }

                var isAdmin = User.IsInRole("Admin");
                if (isAdmin && !userId.HasValue)
                {
                    userId = null;
                }

                var success = await _notificationService.DanhDauDaDocAsync(request.Id, userId);
                if (!success)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy thông báo hoặc không có quyền" });
                }

                return Ok(new { success = true, message = "Đã đánh dấu đã đọc" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đánh dấu thông báo đã đọc");
                return StatusCode(500, new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        /// <summary>
        /// PUT /api/notification/read-all
        /// Đánh dấu tất cả thông báo đã đọc
        /// </summary>
        [HttpPut("read-all")]
        [Authorize]
        public async Task<IActionResult> MarkAllAsRead([FromQuery] int? userId)
        {
            try
            {
                // Kiểm tra nếu là admin
                var isAdmin = User.IsInRole("Admin") || User.IsInRole("NhanVien");
                if (isAdmin)
                {
                    // Admin xem thông báo gửi cho admin (NguoiNhanId = null)
                    userId = null;
                }
                else
                {
                    // Lấy userId từ claim nếu không có trong query
                    if (!userId.HasValue)
                    {
                        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                        if (int.TryParse(userIdClaim, out int parsedUserId))
                        {
                            userId = parsedUserId;
                        }
                    }
                }

                var count = await _notificationService.DanhDauTatCaDaDocAsync(userId);
                return Ok(new { success = true, message = $"Đã đánh dấu {count} thông báo đã đọc" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đánh dấu tất cả thông báo đã đọc");
                return StatusCode(500, new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        /// <summary>
        /// POST /api/notification/send-health-update
        /// Admin gửi thông báo tình trạng sức khỏe thú cưng cho khách hàng
        /// </summary>
        [HttpPost("send-health-update")]
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> SendHealthUpdate([FromBody] SendHealthUpdateRequest request)
        {
            try
            {
                if (request == null || request.ThuCungId <= 0 || string.IsNullOrEmpty(request.TinhTrang))
                {
                    return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ" });
                }

                try
                {
                    await _notificationService.TaoThongBaoTinhTrangSucKhoeAsync(
                        request.ThuCungId,
                        request.TinhTrang,
                        request.MoTaChiTiet,
                        request.HinhAnh
                    );

                    return Ok(new { success = true, message = "Đã gửi thông báo tình trạng sức khỏe thành công" });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi gửi thông báo tình trạng sức khỏe cho ThuCung ID: {ThuCungId}", request.ThuCungId);
                    return Ok(new { success = true, message = "Đã gửi thông báo tình trạng sức khỏe thành công", warning = "Có thể khách hàng chưa có tài khoản để nhận thông báo" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi thông báo tình trạng sức khỏe");
                return StatusCode(500, new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }
    }

    /// <summary>
    /// Request model cho tạo thông báo
    /// </summary>
    public class CreateNotificationRequest
    {
        public int? NguoiNhanId { get; set; }
        public string LoaiThongBao { get; set; } = string.Empty;
        public string? TieuDe { get; set; }
        public string? NoiDung { get; set; }
        public int? DatLichId { get; set; }
        public int? ThuCungId { get; set; }
    }

    /// <summary>
    /// Request model cho đánh dấu đã đọc
    /// </summary>
    public class MarkAsReadRequest
    {
        public int Id { get; set; }
    }

    /// <summary>
    /// Request model cho gửi thông báo tình trạng sức khỏe
    /// </summary>
    public class SendHealthUpdateRequest
    {
        public int ThuCungId { get; set; }
        public string TinhTrang { get; set; } = string.Empty; // Ví dụ: "Khỏe mạnh", "Cần theo dõi", "Cần chăm sóc đặc biệt"
        public string? MoTaChiTiet { get; set; }
        public string? HinhAnh { get; set; } // URL của ảnh đã upload
    }
}

