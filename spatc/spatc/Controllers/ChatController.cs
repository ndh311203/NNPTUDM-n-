using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using spatc.Hubs;
using spatc.Models;
using spatc.Services;
using System.Security.Claims;

namespace spatc.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly ChatService _chatService;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ChatController> _logger;

        public ChatController(
            ChatService chatService,
            IHubContext<ChatHub> hubContext,
            ApplicationDbContext context,
            ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _hubContext = hubContext;
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> SendMessage([FromForm] int? nguoiNhanId, [FromForm] string noiDung)
        {
            try
            {
                var nguoiGuiId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var nguoiGuiTen = User.FindFirst(ClaimTypes.Name)?.Value ?? "Khách";
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var isAdmin = userRole == "Admin" || userRole == "NhanVien";

                if (string.IsNullOrWhiteSpace(noiDung))
                {
                    return Json(new { success = false, message = "Nội dung tin nhắn không được để trống" });
                }

                var chatMessage = await _chatService.LuuTinNhanAsync(nguoiGuiId, nguoiNhanId, noiDung, isAdmin);

                var messageData = new
                {
                    id = chatMessage.Id,
                    nguoiGuiId = chatMessage.NguoiGuiId,
                    nguoiGuiTen = nguoiGuiTen,
                    nguoiNhanId = chatMessage.NguoiNhanId,
                    noiDung = chatMessage.NoiDung,
                    thoiGianGui = chatMessage.ThoiGianGui,
                    isAdmin = chatMessage.IsAdmin
                };

                if (isAdmin)
                {
                    if (nguoiNhanId.HasValue)
                    {
                        await _hubContext.Clients.Group($"user_{nguoiNhanId.Value}").SendAsync("ReceiveMessage", messageData);
                    }
                    else
                    {
                        await _hubContext.Clients.Group("chat_users").SendAsync("ReceiveMessage", messageData);
                    }
                }
                else
                {
                    await _hubContext.Clients.Group("chat_admin").SendAsync("ReceiveMessage", messageData);
                    await _hubContext.Clients.Group($"user_{nguoiGuiId}").SendAsync("ReceiveMessage", messageData);
                }

                return Json(new { success = true, message = messageData });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi tin nhắn");
                return Json(new { success = false, message = "Đã xảy ra lỗi khi gửi tin nhắn" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages(int? nguoiNhanId, int skip = 0, int take = 50)
        {
            try
            {
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var isAdminUser = userRole == "Admin" || userRole == "NhanVien";

                List<ChatMessage> messages;
                if (nguoiNhanId.HasValue)
                {
                    messages = await _chatService.LayTinNhanTheoNguoiDungAsync(currentUserId, nguoiNhanId.Value, skip, take);
                }
                else
                {
                    messages = await _chatService.LayTinNhanAsync(currentUserId, isAdminUser, skip, take);
                }

                var messageList = messages.Select(m => new
                {
                    id = m.Id,
                    nguoiGuiId = m.NguoiGuiId,
                    nguoiGuiTen = m.NguoiGui?.HoTen ?? "Hỗ trợ viên",
                    nguoiNhanId = m.NguoiNhanId,
                    noiDung = m.NoiDung,
                    thoiGianGui = m.ThoiGianGui,
                    isAdmin = m.IsAdmin,
                    isMe = m.NguoiGuiId == currentUserId
                }).OrderBy(m => m.thoiGianGui).ToList();

                return Json(new { success = true, messages = messageList });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tin nhắn");
                return Json(new { success = false, messages = new List<object>() });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetConversations()
        {
            try
            {
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var isAdmin = userRole == "Admin" || userRole == "NhanVien";

                var conversations = await _chatService.LayDanhSachNguoiChatAsync(isAdmin, currentUserId);
                return Json(new { success = true, conversations = conversations });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách cuộc trò chuyện");
                return Json(new { success = false, conversations = new List<object>() });
            }
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> MarkAsRead([FromForm] int nguoiGuiId)
        {
            try
            {
                var nguoiNhanId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                await _chatService.DanhDauDaDocAsync(nguoiGuiId, nguoiNhanId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đánh dấu đã đọc");
                return Json(new { success = false });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            try
            {
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var isAdmin = userRole == "Admin" || userRole == "NhanVien";

                var count = await _chatService.DemTinNhanChuaDocAsync(currentUserId, isAdmin);
                return Json(new { success = true, count = count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đếm tin nhắn chưa đọc");
                return Json(new { success = false, count = 0 });
            }
        }
    }
}

