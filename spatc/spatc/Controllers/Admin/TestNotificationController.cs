using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spatc.Models;
using spatc.Services;

namespace spatc.Controllers.Admin
{
    /// <summary>
    /// Controller test thông báo (tạm thời để debug)
    /// </summary>
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]")]
    public class TestNotificationController : Controller
    {
        private readonly NotificationService _notificationService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TestNotificationController> _logger;

        public TestNotificationController(
            NotificationService notificationService,
            ApplicationDbContext context,
            ILogger<TestNotificationController> logger)
        {
            _notificationService = notificationService;
            _context = context;
            _logger = logger;
        }

        // GET: Admin/TestNotification
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            // Đếm số thông báo trong database
            var totalNotifications = await _context.ThongBaos.CountAsync();
            var unreadNotifications = await _context.ThongBaos.Where(t => !t.DaDoc).CountAsync();
            var adminNotifications = await _context.ThongBaos.Where(t => t.NguoiNhanId == null).CountAsync();
            var customerNotifications = await _context.ThongBaos.Where(t => t.NguoiNhanId != null).CountAsync();

            ViewBag.TotalNotifications = totalNotifications;
            ViewBag.UnreadNotifications = unreadNotifications;
            ViewBag.AdminNotifications = adminNotifications;
            ViewBag.CustomerNotifications = customerNotifications;

            // Lấy 10 thông báo mới nhất
            var recentNotifications = await _context.ThongBaos
                .OrderByDescending(t => t.ThoiGianTao)
                .Take(10)
                .ToListAsync();

            return View("~/Views/Admin/TestNotification/Index.cshtml", recentNotifications);
        }

        // POST: Admin/TestNotification/CreateTest
        [HttpPost]
        [Route("CreateTest")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> CreateTest()
        {
            try
            {
                var testNotification = await _notificationService.TaoThongBaoAsync(
                    LoaiThongBao.ADMIN_BOOKING_NEW,
                    "Test thông báo",
                    "Đây là thông báo test để kiểm tra hệ thống.",
                    nguoiNhanId: null, // Gửi cho admin
                    datLichId: null,
                    thuCungId: null
                );

                return Json(new { 
                    success = true, 
                    message = "Đã tạo thông báo test thành công",
                    notificationId = testNotification.Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo thông báo test");
                return Json(new { 
                    success = false, 
                    message = $"Lỗi: {ex.Message}",
                    stackTrace = ex.StackTrace
                });
            }
        }
    }
}

