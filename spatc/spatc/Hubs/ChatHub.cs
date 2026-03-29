using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace spatc.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
                var isAdmin = userRole == "Admin" || userRole == "NhanVien";

                _logger.LogInformation("ChatHub Client connecting - ConnectionId: {ConnectionId}, UserId: {UserId}, IsAdmin: {IsAdmin}",
                    Context.ConnectionId, userId ?? "null", isAdmin);

                if (!string.IsNullOrEmpty(userId))
                {
                    var userGroup = $"user_{userId}";
                    await Groups.AddToGroupAsync(Context.ConnectionId, userGroup);
                    _logger.LogInformation("Đã thêm connection {ConnectionId} vào group {GroupName}", 
                        Context.ConnectionId, userGroup);
                }

                if (isAdmin)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, "chat_admin");
                    _logger.LogInformation("Đã thêm connection {ConnectionId} vào group chat_admin", Context.ConnectionId);
                }
                else if (!string.IsNullOrEmpty(userId))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, "chat_users");
                    _logger.LogInformation("Đã thêm connection {ConnectionId} vào group chat_users", Context.ConnectionId);
                }

                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi client kết nối ChatHub");
                throw;
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
            var isAdmin = userRole == "Admin" || userRole == "NhanVien";

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
            }

            if (isAdmin)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "chat_admin");
            }
            else if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "chat_users");
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string nguoiNhanId, string noiDung)
        {
            try
            {
                var nguoiGuiId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var nguoiGuiTen = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Khách";
                var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
                var isAdmin = userRole == "Admin" || userRole == "NhanVien";

                if (string.IsNullOrEmpty(nguoiGuiId))
                {
                    _logger.LogWarning("Người gửi không có ID");
                    return;
                }

                var messageData = new
                {
                    nguoiGuiId = int.Parse(nguoiGuiId),
                    nguoiGuiTen = nguoiGuiTen,
                    nguoiNhanId = !string.IsNullOrEmpty(nguoiNhanId) ? int.Parse(nguoiNhanId) : (int?)null,
                    noiDung = noiDung,
                    thoiGianGui = DateTime.Now,
                    isAdmin = isAdmin
                };

                if (isAdmin)
                {
                    if (!string.IsNullOrEmpty(nguoiNhanId))
                    {
                        await Clients.Group($"user_{nguoiNhanId}").SendAsync("ReceiveMessage", messageData);
                        await Clients.Caller.SendAsync("ReceiveMessage", messageData);
                    }
                    else
                    {
                        await Clients.Group("chat_users").SendAsync("ReceiveMessage", messageData);
                    }
                }
                else
                {
                    await Clients.Group("chat_admin").SendAsync("ReceiveMessage", messageData);
                    await Clients.Caller.SendAsync("ReceiveMessage", messageData);
                }

                _logger.LogInformation("Đã gửi tin nhắn từ {NguoiGuiId} đến {NguoiNhanId}", nguoiGuiId, nguoiNhanId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi tin nhắn");
            }
        }
    }
}

