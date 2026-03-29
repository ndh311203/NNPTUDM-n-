using Microsoft.EntityFrameworkCore;
using spatc.Models;

namespace spatc.Services
{
    public class ChatService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ChatService> _logger;

        public ChatService(ApplicationDbContext context, ILogger<ChatService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ChatMessage> LuuTinNhanAsync(
            int nguoiGuiId,
            int? nguoiNhanId,
            string noiDung,
            bool isAdmin)
        {
            var chatMessage = new ChatMessage
            {
                NguoiGuiId = nguoiGuiId,
                NguoiNhanId = nguoiNhanId,
                NoiDung = noiDung,
                LoaiTinNhan = "text",
                DaDoc = false,
                ThoiGianGui = DateTime.Now,
                IsAdmin = isAdmin
            };

            _context.ChatMessages.Add(chatMessage);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Đã lưu tin nhắn - ID: {Id}, Từ: {NguoiGuiId}, Đến: {NguoiNhanId}",
                chatMessage.Id, nguoiGuiId, nguoiNhanId);

            return chatMessage;
        }

        public async Task<List<ChatMessage>> LayTinNhanAsync(int? userId, bool isAdmin, int skip = 0, int take = 50)
        {
            var query = _context.ChatMessages
                .Include(c => c.NguoiGui)
                .Include(c => c.NguoiNhan)
                .AsQueryable();

            if (userId.HasValue)
            {
                if (isAdmin)
                {
                    query = query.Where(c => !c.IsAdmin || c.NguoiNhanId == userId.Value);
                }
                else
                {
                    query = query.Where(c => (c.NguoiGuiId == userId.Value || c.NguoiNhanId == userId.Value) ||
                                           (c.IsAdmin && !c.NguoiNhanId.HasValue));
                }
            }

            return await query
                .OrderByDescending(c => c.ThoiGianGui)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<List<ChatMessage>> LayTinNhanTheoNguoiDungAsync(int nguoiGuiId, int? nguoiNhanId, int skip = 0, int take = 50)
        {
            var query = _context.ChatMessages
                .Include(c => c.NguoiGui)
                .Include(c => c.NguoiNhan)
                .Where(c => (c.NguoiGuiId == nguoiGuiId && c.NguoiNhanId == nguoiNhanId) ||
                           (c.NguoiGuiId == nguoiNhanId && c.NguoiNhanId == nguoiGuiId))
                .OrderByDescending(c => c.ThoiGianGui)
                .Skip(skip)
                .Take(take);

            return await query.ToListAsync();
        }

        public async Task<List<object>> LayDanhSachNguoiChatAsync(bool isAdmin, int currentUserId)
        {
            if (isAdmin)
            {
                var usersWithMessages = await _context.ChatMessages
                    .Where(c => !c.IsAdmin)
                    .Select(c => new { c.NguoiGuiId, c.NguoiGui!.HoTen, c.NguoiGui.Email, c.ThoiGianGui })
                    .GroupBy(c => c.NguoiGuiId)
                    .Select(g => new
                    {
                        UserId = g.Key,
                        HoTen = g.First().HoTen,
                        Email = g.First().Email,
                        LastMessageTime = g.Max(c => c.ThoiGianGui),
                        UnreadCount = _context.ChatMessages
                            .Count(c => c.NguoiGuiId == g.Key && 
                                      !c.IsAdmin &&
                                      (c.NguoiNhanId == null || c.NguoiNhanId == currentUserId) &&
                                      !c.DaDoc)
                    })
                    .OrderByDescending(x => x.LastMessageTime)
                    .ToListAsync();

                return usersWithMessages.Cast<object>().ToList();
            }
            else
            {
                var adminIds = await _context.TaiKhoans
                    .Where(t => t.VaiTro == "Admin" || t.VaiTro == "NhanVien")
                    .Select(t => t.Id)
                    .ToListAsync();

                var conversations = new List<object>();

                foreach (var adminId in adminIds)
                {
                    var lastMessage = await _context.ChatMessages
                        .Where(c => (c.NguoiGuiId == currentUserId && c.NguoiNhanId == adminId) ||
                                   (c.NguoiGuiId == adminId && c.NguoiNhanId == currentUserId))
                        .OrderByDescending(c => c.ThoiGianGui)
                        .FirstOrDefaultAsync();

                    if (lastMessage != null)
                    {
                        var admin = await _context.TaiKhoans.FindAsync(adminId);
                        var unreadCount = await _context.ChatMessages
                            .CountAsync(c => c.NguoiGuiId == adminId && 
                                           c.NguoiNhanId == currentUserId && 
                                           !c.DaDoc);

                        conversations.Add(new
                        {
                            UserId = adminId,
                            HoTen = admin?.HoTen ?? "Hỗ trợ viên",
                            Email = admin?.Email,
                            LastMessageTime = lastMessage.ThoiGianGui,
                            UnreadCount = unreadCount
                        });
                    }
                }

                return conversations.OrderByDescending(c => ((dynamic)c).LastMessageTime).ToList();
            }
        }

        public async Task<int> DemTinNhanChuaDocAsync(int userId, bool isAdmin)
        {
            if (isAdmin)
            {
                return await _context.ChatMessages
                    .CountAsync(c => !c.DaDoc && !c.IsAdmin && (c.NguoiNhanId == null || c.NguoiNhanId == userId));
            }
            else
            {
                return await _context.ChatMessages
                    .CountAsync(c => !c.DaDoc && c.IsAdmin && (c.NguoiNhanId == null || c.NguoiNhanId == userId));
            }
        }

        public async Task DanhDauDaDocAsync(int nguoiGuiId, int nguoiNhanId)
        {
            var messages = await _context.ChatMessages
                .Where(c => c.NguoiGuiId == nguoiGuiId && c.NguoiNhanId == nguoiNhanId && !c.DaDoc)
                .ToListAsync();

            foreach (var message in messages)
            {
                message.DaDoc = true;
            }

            await _context.SaveChangesAsync();
        }
    }
}

