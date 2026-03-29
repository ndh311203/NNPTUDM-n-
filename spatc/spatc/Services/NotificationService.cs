using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using spatc.Hubs;
using spatc.Models;

namespace spatc.Services
{
    /// <summary>
    /// Service xử lý logic thông báo
    /// </summary>
    public class NotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            ApplicationDbContext context,
            IHubContext<NotificationHub> hubContext,
            ILogger<NotificationService> logger)
        {
            _context = context;
            _hubContext = hubContext;
            _logger = logger;
        }

        /// <summary>
        /// Tạo thông báo và gửi realtime
        /// </summary>
        public async Task<ThongBao> TaoThongBaoAsync(
            string loaiThongBao,
            string tieuDe,
            string noiDung,
            int? nguoiNhanId = null,
            int? datLichId = null,
            int? thuCungId = null,
            string? hinhAnh = null)
        {
            var thongBao = new ThongBao
            {
                LoaiThongBao = loaiThongBao,
                TieuDe = tieuDe,
                NoiDung = noiDung,
                NguoiNhanId = nguoiNhanId,
                DatLichId = datLichId,
                ThuCungId = thuCungId,
                HinhAnh = hinhAnh,
                DaDoc = false,
                ThoiGianTao = DateTime.Now
            };

            _logger.LogInformation("Đang lưu thông báo vào database...");
            _context.ThongBaos.Add(thongBao);
            await _context.SaveChangesAsync();
            _logger.LogInformation("✅ Đã lưu thông báo vào database - ID: {Id}", thongBao.Id);

            // Gửi realtime notification
            _logger.LogInformation("Đang gửi thông báo realtime qua SignalR...");
            await GuiThongBaoRealtimeAsync(thongBao);
            _logger.LogInformation("✅ Đã gửi thông báo realtime");

            return thongBao;
        }

        /// <summary>
        /// Gửi thông báo realtime qua SignalR
        /// </summary>
        private async Task GuiThongBaoRealtimeAsync(ThongBao thongBao)
        {
            try
            {
                var notificationData = new
                {
                    id = thongBao.Id,
                    loaiThongBao = thongBao.LoaiThongBao,
                    tieuDe = thongBao.TieuDe,
                    noiDung = thongBao.NoiDung,
                    datLichId = thongBao.DatLichId,
                    thuCungId = thongBao.ThuCungId,
                    hinhAnh = thongBao.HinhAnh,
                    daDoc = thongBao.DaDoc,
                    thoiGianTao = thongBao.ThoiGianTao
                };

                // Nếu có người nhận cụ thể, gửi đến user đó
                if (thongBao.NguoiNhanId.HasValue)
                {
                    var groupName = $"user_{thongBao.NguoiNhanId.Value}";
                    _logger.LogInformation("Gửi thông báo đến group: {GroupName}", groupName);
                    await _hubContext.Clients.Group(groupName)
                        .SendAsync("NhanThongBao", notificationData);
                    _logger.LogInformation("Đã gửi thông báo realtime đến user {UserId}: {TieuDe}", 
                        thongBao.NguoiNhanId.Value, thongBao.TieuDe);
                }
                else
                {
                    // Nếu không có người nhận cụ thể, gửi cho tất cả admin
                    _logger.LogInformation("Gửi thông báo đến group: admin");
                    await _hubContext.Clients.Group("admin")
                        .SendAsync("NhanThongBao", notificationData);
                    _logger.LogInformation("Đã gửi thông báo realtime đến tất cả admin: {TieuDe}", thongBao.TieuDe);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi thông báo realtime: {TieuDe}", thongBao.TieuDe);
            }
        }

        /// <summary>
        /// Tạo thông báo khi khách đặt lịch mới (gửi đến Admin)
        /// </summary>
        public async Task TaoThongBaoDatLichMoiAsync(DatLich datLich)
        {
            try
            {
                _logger.LogInformation("=== NotificationService.TaoThongBaoDatLichMoiAsync - BẮT ĐẦU ===");
                _logger.LogInformation("DatLich ID: {Id}", datLich?.Id ?? 0);

                if (datLich == null)
                {
                    _logger.LogError("❌ DatLich là NULL!");
                    return;
                }

                // Load lại datLich với đầy đủ thông tin
                _logger.LogInformation("Đang load DatLich với đầy đủ thông tin...");
                var datLichFull = await _context.DatLiches
                    .Include(d => d.ThuCung)
                        .ThenInclude(t => t != null ? t.KhachHang : null!)
                    .Include(d => d.PhongKhachSan)
                    .Include(d => d.DatLichDichVuGroomings)
                        .ThenInclude(dlg => dlg.DichVuGrooming)
                    .FirstOrDefaultAsync(d => d.Id == datLich.Id);

                if (datLichFull == null)
                {
                    _logger.LogError("❌ Không tìm thấy DatLich ID: {DatLichId}", datLich.Id);
                    return;
                }

                if (datLichFull.ThuCung == null)
                {
                    _logger.LogError("❌ DatLich ID {DatLichId} không có ThuCung", datLich.Id);
                    return;
                }

                if (datLichFull.ThuCung.KhachHang == null)
                {
                    _logger.LogError("❌ ThuCung ID {ThuCungId} không có KhachHang", datLichFull.ThuCungId);
                    return;
                }

                var tenKhachHang = datLichFull.ThuCung.KhachHang.HoTen ?? "N/A";
                var tenThuCung = datLichFull.ThuCung.TenThuCung ?? "N/A";
                var thoiGian = datLichFull.ThoiGianHen.ToString("dd/MM/yyyy HH:mm");

                _logger.LogInformation("Thông tin đặt lịch - Khách hàng: {KhachHang}, Thú cưng: {ThuCung}, Thời gian: {ThoiGian}", 
                    tenKhachHang, tenThuCung, thoiGian);

                // Xác định loại dịch vụ
                string loaiDichVu = "Dịch vụ";
                if (datLichFull.PhongKhachSanId.HasValue)
                {
                    loaiDichVu = "Khách sạn";
                }
                else if (datLichFull.DatLichDichVuGroomings != null && datLichFull.DatLichDichVuGroomings.Any())
                {
                    loaiDichVu = "Grooming";
                }

                var tieuDe = "Đặt lịch mới";
                var noiDung = $"Khách {tenKhachHang} đã đặt lịch {loaiDichVu} cho {tenThuCung} lúc {thoiGian}.";

                _logger.LogInformation("Đang tạo thông báo - Tiêu đề: {TieuDe}, Nội dung: {NoiDung}", tieuDe, noiDung);

                var thongBao = await TaoThongBaoAsync(
                    LoaiThongBao.ADMIN_BOOKING_NEW,
                    tieuDe,
                    noiDung,
                    nguoiNhanId: null, // null = gửi cho tất cả admin
                    datLichId: datLichFull.Id,
                    thuCungId: datLichFull.ThuCungId
                );

                _logger.LogInformation("✅ Đã tạo thông báo thành công - ID: {ThongBaoId}", thongBao?.Id ?? 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ LỖI KHI TẠO THÔNG BÁO ĐẶT LỊCH");
                _logger.LogError("Exception Type: {Type}", ex.GetType().Name);
                _logger.LogError("Exception Message: {Message}", ex.Message);
                _logger.LogError("Exception StackTrace: {StackTrace}", ex.StackTrace);
                if (ex.InnerException != null)
                {
                    _logger.LogError("Inner Exception: {Message}", ex.InnerException.Message);
                }
                throw; // Re-throw để controller có thể catch
            }
        }

        /// <summary>
        /// Tạo thông báo khi admin cập nhật trạng thái thú cưng (gửi đến Khách hàng)
        /// </summary>
        public async Task TaoThongBaoTrangThaiThuCungAsync(
            int thuCungId,
            string trangThai,
            string? moTaChiTiet = null)
        {
            try
            {
                var thuCung = await _context.ThuCungs
                    .Include(t => t.KhachHang)
                        .ThenInclude(k => k.TaiKhoan)
                    .FirstOrDefaultAsync(t => t.Id == thuCungId);

                if (thuCung == null || thuCung.KhachHang == null)
                {
                    _logger.LogWarning("Không tìm thấy thú cưng hoặc khách hàng cho ThuCung ID: {ThuCungId}", thuCungId);
                    return;
                }

                // Lấy DatLich gần nhất của thú cưng này
                var datLich = await _context.DatLiches
                    .Where(d => d.ThuCungId == thuCungId)
                    .OrderByDescending(d => d.ThoiGianHen)
                    .FirstOrDefaultAsync();

                var tenThuCung = thuCung.TenThuCung;
                var nguoiNhanId = thuCung.KhachHang.TaiKhoanId;

                if (!nguoiNhanId.HasValue)
                {
                    _logger.LogWarning("Khách hàng ID {KhachHangId} chưa có tài khoản, không thể gửi thông báo", 
                        thuCung.KhachHang.Id);
                    return;
                }

                string loaiThongBao;
                string tieuDe;
                string noiDung;

                switch (trangThai.ToUpper())
                {
                    case "CHECKIN":
                        loaiThongBao = LoaiThongBao.PET_CHECKIN;
                        tieuDe = "Thú cưng đã được nhận";
                        noiDung = $"Bé {tenThuCung} đã được check-in tại spa.";
                        break;

                    case "BATHING":
                    case "DRYING":
                    case "TRIMMING":
                        loaiThongBao = LoaiThongBao.PET_IN_SERVICE;
                        tieuDe = "Đang thực hiện dịch vụ";
                        var dichVuHienTai = trangThai == "BATHING" ? "tắm" : 
                                          trangThai == "DRYING" ? "sấy khô" : "cắt tỉa";
                        noiDung = $"Bé {tenThuCung} đang {dichVuHienTai}.";
                        if (!string.IsNullOrEmpty(moTaChiTiet))
                        {
                            noiDung += $" {moTaChiTiet}";
                        }
                        break;

                    case "DONE":
                        loaiThongBao = LoaiThongBao.PET_DONE;
                        tieuDe = "Hoàn tất dịch vụ";
                        noiDung = $"Bé {tenThuCung} đã hoàn tất dịch vụ. Bạn có thể đến đón.";
                        break;

                    case "HOTEL_DAILY":
                        loaiThongBao = LoaiThongBao.PET_HOTEL_DAILY;
                        tieuDe = "Báo cáo hằng ngày";
                        noiDung = $"Bé {tenThuCung} hôm nay ăn tốt, chơi ngoan.";
                        if (!string.IsNullOrEmpty(moTaChiTiet))
                        {
                            noiDung = moTaChiTiet;
                        }
                        break;

                    case "ISSUE":
                        loaiThongBao = LoaiThongBao.PET_ISSUE;
                        tieuDe = "Có vấn đề";
                        noiDung = $"Bé {tenThuCung} có dấu hiệu bất thường. Vui lòng liên hệ gấp.";
                        if (!string.IsNullOrEmpty(moTaChiTiet))
                        {
                            noiDung = moTaChiTiet;
                        }
                        break;

                    default:
                        _logger.LogWarning("Trạng thái không hợp lệ: {TrangThai}", trangThai);
                        return;
                }

                await TaoThongBaoAsync(
                    loaiThongBao,
                    tieuDe,
                    noiDung,
                    nguoiNhanId: nguoiNhanId.Value,
                    datLichId: datLich?.Id,
                    thuCungId: thuCungId
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo thông báo trạng thái thú cưng cho ThuCung ID: {ThuCungId}", thuCungId);
            }
        }

        /// <summary>
        /// Lấy danh sách thông báo của người dùng
        /// </summary>
        public async Task<List<ThongBao>> LayDanhSachThongBaoAsync(int? nguoiNhanId, int skip = 0, int take = 20)
        {
            var query = _context.ThongBaos.AsQueryable();

            if (nguoiNhanId.HasValue)
            {
                query = query.Where(t => t.NguoiNhanId == nguoiNhanId.Value);
            }
            else
            {
                // Nếu không có nguoiNhanId, lấy thông báo gửi cho admin (NguoiNhanId = null)
                query = query.Where(t => t.NguoiNhanId == null);
            }

            return await query
                .OrderByDescending(t => t.ThoiGianTao)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        /// <summary>
        /// Đếm số thông báo chưa đọc
        /// </summary>
        public async Task<int> DemThongBaoChuaDocAsync(int? nguoiNhanId)
        {
            var query = _context.ThongBaos.Where(t => !t.DaDoc);

            if (nguoiNhanId.HasValue)
            {
                query = query.Where(t => t.NguoiNhanId == nguoiNhanId.Value);
            }
            else
            {
                query = query.Where(t => t.NguoiNhanId == null);
            }

            return await query.CountAsync();
        }

        /// <summary>
        /// Đánh dấu thông báo đã đọc
        /// </summary>
        public async Task<bool> DanhDauDaDocAsync(int thongBaoId, int? nguoiNhanId)
        {
            var thongBao = await _context.ThongBaos.FindAsync(thongBaoId);
            if (thongBao == null)
            {
                return false;
            }

            // Kiểm tra quyền: chỉ người nhận mới được đánh dấu đã đọc
            if (thongBao.NguoiNhanId.HasValue && thongBao.NguoiNhanId != nguoiNhanId)
            {
                return false;
            }

            thongBao.DaDoc = true;
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Đánh dấu tất cả thông báo đã đọc
        /// </summary>
        public async Task<int> DanhDauTatCaDaDocAsync(int? nguoiNhanId)
        {
            var query = _context.ThongBaos.Where(t => !t.DaDoc);

            if (nguoiNhanId.HasValue)
            {
                query = query.Where(t => t.NguoiNhanId == nguoiNhanId.Value);
            }
            else
            {
                query = query.Where(t => t.NguoiNhanId == null);
            }

            var thongBaos = await query.ToListAsync();
            foreach (var tb in thongBaos)
            {
                tb.DaDoc = true;
            }

            await _context.SaveChangesAsync();
            return thongBaos.Count;
        }

        /// <summary>
        /// Tạo thông báo khi khách hàng đặt hàng mới (gửi đến Admin)
        /// </summary>
        public async Task TaoThongBaoDonHangMoiAsync(ShopOrder order)
        {
            try
            {
                // Load lại order với ShopOrderItems để đếm số lượng
                var orderFull = await _context.ShopOrders
                    .Include(o => o.ShopOrderItems)
                    .FirstOrDefaultAsync(o => o.Id == order.Id);

                if (orderFull == null)
                {
                    _logger.LogWarning("Không tìm thấy ShopOrder ID: {OrderId}", order.Id);
                    return;
                }

                var soLuongSanPham = orderFull.ShopOrderItems?.Sum(i => i.Quantity) ?? 0;
                var tongTien = orderFull.TotalAmount.ToString("N0");

                var tieuDe = "Đơn hàng mới";
                var noiDung = $"Khách {orderFull.CustomerName} ({orderFull.Phone}) đã đặt {soLuongSanPham} sản phẩm với tổng tiền {tongTien} đ. " +
                             $"Phương thức thanh toán: {orderFull.PaymentMethod}.";

                await TaoThongBaoAsync(
                    LoaiThongBao.ADMIN_ORDER_NEW,
                    tieuDe,
                    noiDung,
                    nguoiNhanId: null, // null = gửi cho tất cả admin
                    datLichId: null,
                    thuCungId: null
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo thông báo đơn hàng mới cho ShopOrder ID: {OrderId}", order.Id);
            }
        }

        /// <summary>
        /// Tạo thông báo tình trạng sức khỏe thú cưng cho khách hàng (Admin gửi)
        /// </summary>
        public async Task TaoThongBaoTinhTrangSucKhoeAsync(
            int thuCungId,
            string tinhTrang,
            string? moTaChiTiet = null,
            string? hinhAnh = null)
        {
            try
            {
                var thuCung = await _context.ThuCungs
                    .Include(t => t.KhachHang)
                        .ThenInclude(k => k.TaiKhoan)
                    .FirstOrDefaultAsync(t => t.Id == thuCungId);

                if (thuCung == null || thuCung.KhachHang == null)
                {
                    _logger.LogWarning("Không tìm thấy thú cưng hoặc khách hàng cho ThuCung ID: {ThuCungId}", thuCungId);
                    return;
                }

                var tenThuCung = thuCung.TenThuCung;
                
                // Lấy TaiKhoanId từ KhachHang.TaiKhoanId hoặc từ KhachHang.TaiKhoan.Id
                int? nguoiNhanId = thuCung.KhachHang.TaiKhoanId;
                if (!nguoiNhanId.HasValue && thuCung.KhachHang.TaiKhoan != null)
                {
                    nguoiNhanId = thuCung.KhachHang.TaiKhoan.Id;
                }

                // Nếu vẫn chưa có, thử tìm tài khoản qua email của khách hàng
                if (!nguoiNhanId.HasValue && !string.IsNullOrEmpty(thuCung.KhachHang.Email))
                {
                    var taiKhoan = await _context.TaiKhoans
                        .FirstOrDefaultAsync(t => t.Email == thuCung.KhachHang.Email && 
                                                 t.VaiTro != "Admin" && 
                                                 t.VaiTro != "NhanVien");
                    if (taiKhoan != null)
                    {
                        nguoiNhanId = taiKhoan.Id;
                        // Cập nhật TaiKhoanId cho khách hàng để lần sau không cần tìm lại
                        thuCung.KhachHang.TaiKhoanId = taiKhoan.Id;
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Đã liên kết tài khoản {TaiKhoanId} với khách hàng {KhachHangId} qua email",
                            taiKhoan.Id, thuCung.KhachHang.Id);
                    }
                }

                if (!nguoiNhanId.HasValue)
                {
                    _logger.LogWarning("Khách hàng ID {KhachHangId} (Email: {Email}) chưa có tài khoản, không thể gửi thông báo", 
                        thuCung.KhachHang.Id, thuCung.KhachHang.Email ?? "N/A");
                    return;
                }

                _logger.LogInformation("Gửi thông báo sức khỏe cho khách hàng - ThuCungId: {ThuCungId}, NguoiNhanId: {NguoiNhanId}", 
                    thuCungId, nguoiNhanId.Value);

                // Lấy DatLich gần nhất của thú cưng này
                var datLich = await _context.DatLiches
                    .Where(d => d.ThuCungId == thuCungId)
                    .OrderByDescending(d => d.ThoiGianHen)
                    .FirstOrDefaultAsync();

                var tieuDe = "Tình trạng sức khỏe thú cưng";
                var noiDung = $"Bé {tenThuCung} hiện tại: {tinhTrang}.";
                if (!string.IsNullOrEmpty(moTaChiTiet))
                {
                    noiDung += $" {moTaChiTiet}";
                }

                await TaoThongBaoAsync(
                    LoaiThongBao.PET_HEALTH_UPDATE,
                    tieuDe,
                    noiDung,
                    nguoiNhanId: nguoiNhanId.Value,
                    datLichId: datLich?.Id,
                    thuCungId: thuCungId,
                    hinhAnh: hinhAnh
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo thông báo tình trạng sức khỏe cho ThuCung ID: {ThuCungId}", thuCungId);
            }
        }
    }
}

