using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spatc.Models;
using spatc.Services;

namespace spatc.Controllers.Admin
{
    /// <summary>
    /// Controller debug thông báo
    /// </summary>
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]")]
    public class DebugNotificationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly NotificationService _notificationService;
        private readonly ILogger<DebugNotificationController> _logger;

        public DebugNotificationController(
            ApplicationDbContext context,
            NotificationService notificationService,
            ILogger<DebugNotificationController> logger)
        {
            _context = context;
            _notificationService = notificationService;
            _logger = logger;
        }

        // GET: Admin/DebugNotification
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            // Lấy thông tin debug
            var totalNotifications = await _context.ThongBaos.CountAsync();
            var recentNotifications = await _context.ThongBaos
                .OrderByDescending(t => t.ThoiGianTao)
                .Take(20)
                .ToListAsync();

            var recentBookings = await _context.DatLiches
                .Include(d => d.ThuCung)
                    .ThenInclude(t => t != null ? t.KhachHang : null!)
                .OrderByDescending(d => d.ThoiGianHen)
                .Take(10)
                .ToListAsync();

            ViewBag.TotalNotifications = totalNotifications;
            ViewBag.RecentNotifications = recentNotifications;
            ViewBag.RecentBookings = recentBookings;

            return View("~/Views/Admin/DebugNotification/Index.cshtml");
        }

        // POST: Admin/DebugNotification/TestCreate
        [HttpPost]
        [Route("TestCreate")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> TestCreate()
        {
            try
            {
                _logger.LogInformation("=== BẮT ĐẦU TEST TẠO THÔNG BÁO ===");
                
                var testNotification = await _notificationService.TaoThongBaoAsync(
                    LoaiThongBao.ADMIN_BOOKING_NEW,
                    "Test thông báo từ Debug",
                    "Đây là thông báo test để kiểm tra hệ thống SignalR.",
                    nguoiNhanId: null, // Gửi cho admin
                    datLichId: null,
                    thuCungId: null
                );

                _logger.LogInformation("✅ Đã tạo thông báo test ID: {Id}", testNotification.Id);

                return Json(new { 
                    success = true, 
                    message = "Đã tạo thông báo test thành công",
                    notificationId = testNotification.Id,
                    notification = new {
                        id = testNotification.Id,
                        tieuDe = testNotification.TieuDe,
                        noiDung = testNotification.NoiDung,
                        thoiGianTao = testNotification.ThoiGianTao
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi tạo thông báo test");
                return Json(new { 
                    success = false, 
                    message = $"Lỗi: {ex.Message}",
                    stackTrace = ex.StackTrace
                });
            }
        }

        // POST: Admin/DebugNotification/RecreateForBooking
        [HttpPost]
        [Route("RecreateForBooking")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> RecreateForBooking([FromBody] int datLichId)
        {
            try
            {
                _logger.LogInformation("=== TẠO LẠI THÔNG BÁO CHO ĐẶT LỊCH ID: {DatLichId} ===", datLichId);

                var datLich = await _context.DatLiches
                    .Include(d => d.ThuCung)
                        .ThenInclude(t => t != null ? t.KhachHang : null!)
                    .Include(d => d.PhongKhachSan)
                    .Include(d => d.DatLichDichVuGroomings)
                        .ThenInclude(dlg => dlg.DichVuGrooming)
                    .FirstOrDefaultAsync(d => d.Id == datLichId);

                if (datLich == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đặt lịch" });
                }

                await _notificationService.TaoThongBaoDatLichMoiAsync(datLich);

                return Json(new { 
                    success = true, 
                    message = "Đã tạo lại thông báo cho đặt lịch thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi tạo lại thông báo cho đặt lịch");
                return Json(new { 
                    success = false, 
                    message = $"Lỗi: {ex.Message}"
                });
            }
        }
    }
}

