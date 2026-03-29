using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace spatc.Hubs
{
    /// <summary>
    /// SignalR Hub cho thông báo realtime
    /// </summary>
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Khi client kết nối, thêm vào group theo user ID
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            try
            {
                // Lấy userId từ query string hoặc từ claims
                var userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();
                if (string.IsNullOrEmpty(userId))
                {
                    // Thử lấy từ claims nếu có
                    userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                }

                _logger.LogInformation("SignalR Client connecting - ConnectionId: {ConnectionId}, UserId: {UserId}", 
                    Context.ConnectionId, userId ?? "null");

                if (!string.IsNullOrEmpty(userId))
                {
                    var groupName = $"user_{userId}";
                    await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                    _logger.LogInformation("Đã thêm connection {ConnectionId} vào group {GroupName}", 
                        Context.ConnectionId, groupName);
                }
                
                // Thêm vào group admin nếu là admin
                var isAdmin = Context.GetHttpContext()?.Request.Query["isAdmin"].ToString() == "true";
                if (!isAdmin)
                {
                    // Kiểm tra từ claims
                    var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
                    isAdmin = userRole == "Admin" || userRole == "NhanVien";
                }

                if (isAdmin)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, "admin");
                    _logger.LogInformation("Đã thêm connection {ConnectionId} vào group admin", Context.ConnectionId);
                }

                await base.OnConnectedAsync();
                _logger.LogInformation("SignalR Client connected successfully - ConnectionId: {ConnectionId}", Context.ConnectionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi client kết nối SignalR");
                throw;
            }
        }

        /// <summary>
        /// Khi client ngắt kết nối, xóa khỏi group
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();
            if (string.IsNullOrEmpty(userId))
            {
                userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
            }

            var isAdmin = Context.GetHttpContext()?.Request.Query["isAdmin"].ToString() == "true";
            if (!isAdmin)
            {
                var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
                isAdmin = userRole == "Admin" || userRole == "NhanVien";
            }

            if (isAdmin)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "admin");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}

