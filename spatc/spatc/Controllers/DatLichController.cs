using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spatc.Models;
using Microsoft.Extensions.Logging;
using spatc.Services;

namespace spatc.Controllers
{
    // Controller cho khách hàng đặt lịch - không cần đăng nhập
    public class DatLichController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DatLichController> _logger;
        private readonly NotificationService _notificationService;

        public DatLichController(
            ApplicationDbContext context, 
            ILogger<DatLichController> logger,
            NotificationService notificationService)
        {
            _context = context;
            _logger = logger;
            _notificationService = notificationService;
        }

        // GET: DatLich/TestSubmit - Test form submit
        [HttpGet]
        [Route("/DatLich/TestSubmit")]
        public IActionResult TestSubmit()
        {
            return Content("✅ Test endpoint hoạt động! Form có thể submit đến đây.", "text/plain");
        }

        // POST: DatLich/TestSubmit - Test form submit
        [HttpPost]
        [Route("/DatLich/TestSubmit")]
        [IgnoreAntiforgeryToken]
        public IActionResult TestSubmitPost()
        {
            _logger.LogInformation("=== TEST SUBMIT - NHẬN ĐƯỢC REQUEST ===");
            _logger.LogInformation("Request.Method: {Method}", Request.Method);
            _logger.LogInformation("Request.Path: {Path}", Request.Path);
            _logger.LogInformation("Request.Form.Count: {Count}", Request.Form?.Count ?? 0);
            
            return Content("✅ Test POST endpoint hoạt động! Form đã submit thành công.", "text/plain");
        }

        // GET: DatLich/TestDatabase - Test kết nối database
        [HttpGet]
        [Route("DatLich/TestDatabase")]
        public async Task<IActionResult> TestDatabase()
        {
            try
            {
                _logger.LogInformation("=== TEST DATABASE CONNECTION ===");
                
                // Test 1: Đếm số khách hàng
                var khachHangCount = await _context.KhachHangs.CountAsync();
                _logger.LogInformation($"Số khách hàng trong DB: {khachHangCount}");
                
                // Test 2: Đếm số thú cưng
                var thuCungCount = await _context.ThuCungs.CountAsync();
                _logger.LogInformation($"Số thú cưng trong DB: {thuCungCount}");
                
                // Test 3: Đếm số đặt lịch
                var datLichCount = await _context.DatLiches.CountAsync();
                _logger.LogInformation($"Số đặt lịch trong DB: {datLichCount}");
                
                // Test 4: Thử tạo một khách hàng test
                var testKhachHang = new KhachHang
                {
                    HoTen = "Test Customer",
                    SoDienThoai = $"TEST_{DateTime.Now.Ticks}",
                    NgayTao = DateTime.Now
                };
                _context.KhachHangs.Add(testKhachHang);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"✅ Đã tạo khách hàng test - ID: {testKhachHang.Id}");
                
                // Xóa khách hàng test
                _context.KhachHangs.Remove(testKhachHang);
                await _context.SaveChangesAsync();
                _logger.LogInformation("✅ Đã xóa khách hàng test");
                
                return Json(new { 
                    success = true, 
                    message = "Database connection OK",
                    khachHangCount,
                    thuCungCount,
                    datLichCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ LỖI KHI TEST DATABASE");
                return Json(new { 
                    success = false, 
                    message = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        // GET: DatLich - Trang đặt lịch cho khách hàng
        public async Task<IActionResult> Index(int? dichVuId)
        {
            try
        {
            // Lấy dịch vụ Grooming
            var groomingServices = await _context.DichVuGroomings
                .Where(d => d.TrangThai == true)
                .OrderBy(d => d.Gia)
                .ToListAsync();

            // Lấy phòng khách sạn (chỉ lấy phòng trống)
            var phongKhachSans = await _context.PhongKhachSans
                .Where(p => p.TrangThai == "Trống")
                .OrderBy(p => p.GiaTheoNgay)
                .ToListAsync();

            // Nếu có dichVuId từ trang chủ, tìm dịch vụ Grooming tương ứng
            List<int> selectedGroomingIds = new List<int>();
            if (dichVuId.HasValue)
            {
                var dichVu = await _context.DichVus.FirstOrDefaultAsync(d => d.Id == dichVuId.Value && d.TrangThai == true);
                if (dichVu != null)
                {
                    var matchingGrooming = groomingServices
                        .FirstOrDefault(g => g.TenDichVu.ToLower().Trim() == dichVu.TenDichVu.ToLower().Trim());
                    
                    if (matchingGrooming != null)
                    {
                        selectedGroomingIds.Add(matchingGrooming.Id);
                    }
                }
            }

            ViewBag.GroomingServices = groomingServices;
            ViewBag.PhongKhachSans = phongKhachSans;
                ViewBag.SelectedGroomingIds = selectedGroomingIds;
                
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải dữ liệu cho DatLich/Index");
                TempData["Error"] = "Có lỗi xảy ra khi tải dữ liệu. Vui lòng thử lại sau!";
                ViewBag.GroomingServices = new List<DichVuGrooming>();
                ViewBag.PhongKhachSans = new List<PhongKhachSan>();
                ViewBag.SelectedGroomingIds = new List<int>();
            return View();
            }
        }

        // POST: DatLich/Create - Tạo đặt lịch cho khách hàng
        [HttpPost]
        [Route("/DatLich/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            int[]? DichVuGroomingIds,
            int? PhongKhachSanId,
            string TenThuCung, string Loai, string Giong, int? Tuoi, decimal? CanNang, string GhiChuSucKhoe,
            string HoTen, string SoDienThoai, string Email, string DiaChi,
            string ThoiGianHen, string ThoiGianKetThuc, string GhiChu, string GhiChuDacBiet)
        {
            _logger.LogInformation("=== KHÁCH HÀNG ĐẶT LỊCH - BẮT ĐẦU ===");
            _logger.LogInformation("HoTen: {HoTen}, SoDienThoai: {SoDienThoai}, TenThuCung: {TenThuCung}", HoTen, SoDienThoai, TenThuCung);
            _logger.LogInformation("Loai: {Loai}, ThoiGianHen: {ThoiGianHen}", Loai, ThoiGianHen);
            _logger.LogInformation("DichVuGroomingIds: {Ids}, PhongKhachSanId: {PhongId}", 
                DichVuGroomingIds != null ? string.Join(",", DichVuGroomingIds) : "null", PhongKhachSanId);
            
            // Log tất cả form data
            _logger.LogInformation("=== FORM DATA ===");
            _logger.LogInformation("Request.Method: {Method}", Request.Method);
            _logger.LogInformation("Request.Path: {Path}", Request.Path);
            _logger.LogInformation("Request.ContentType: {ContentType}", Request.ContentType);
            _logger.LogInformation("Request.Form.Count: {Count}", Request.Form?.Count ?? 0);
            
            // Log ModelState errors nếu có
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("=== MODELSTATE ERRORS ===");
                foreach (var error in ModelState)
                {
                    foreach (var err in error.Value.Errors)
                    {
                        _logger.LogWarning("Key: {Key}, Error: {Error}", error.Key, err.ErrorMessage);
                    }
                }
            }
            
            try
            {
                // Validate dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(HoTen) || string.IsNullOrWhiteSpace(SoDienThoai))
                {
                    TempData["Error"] = "Vui lòng nhập đầy đủ thông tin khách hàng!";
                    return RedirectToAction("Index");
                }

                if (string.IsNullOrWhiteSpace(TenThuCung) || string.IsNullOrWhiteSpace(Loai))
                {
                    TempData["Error"] = "Vui lòng nhập đầy đủ thông tin thú cưng!";
                    return RedirectToAction("Index");
                }

                if (string.IsNullOrWhiteSpace(ThoiGianHen))
                {
                    TempData["Error"] = "Vui lòng chọn thời gian hẹn!";
                    return RedirectToAction("Index");
                }

                if ((DichVuGroomingIds == null || DichVuGroomingIds.Length == 0) && !PhongKhachSanId.HasValue)
                {
                    TempData["Error"] = "Vui lòng chọn ít nhất một dịch vụ Grooming hoặc phòng khách sạn!";
                    return RedirectToAction("Index");
                }

                // Parse thời gian
                if (!DateTime.TryParse(ThoiGianHen, out DateTime thoiGianHenParsed))
                {
                    TempData["Error"] = "Thời gian hẹn không hợp lệ!";
                    return RedirectToAction("Index");
                }

                DateTime? thoiGianKetThucParsed = null;
                if (!string.IsNullOrWhiteSpace(ThoiGianKetThuc) && DateTime.TryParse(ThoiGianKetThuc, out DateTime parsedEnd))
                {
                    thoiGianKetThucParsed = parsedEnd;
                }

                // Sử dụng transaction để đảm bảo tất cả dữ liệu được lưu cùng lúc
                using var transaction = await _context.Database.BeginTransactionAsync();
                int datLichId = 0;
                DatLich? datLich = null;
                
                try
                {
                    _logger.LogInformation("=== BẮT ĐẦU TRANSACTION ===");
                    
                    // 1. Tìm hoặc tạo khách hàng - LƯU NGAY để có ID
                    _logger.LogInformation("Bước 1: Tìm hoặc tạo khách hàng với SĐT: {SoDienThoai}", SoDienThoai);
                    var khachHang = await _context.KhachHangs
                        .FirstOrDefaultAsync(k => k.SoDienThoai == SoDienThoai);

                    if (khachHang == null)
                    {
                        khachHang = new KhachHang
                        {
                            HoTen = HoTen,
                            SoDienThoai = SoDienThoai,
                            Email = Email,
                            DiaChi = DiaChi,
                            NgayTao = DateTime.Now
                        };
                        _context.KhachHangs.Add(khachHang);
                        await _context.SaveChangesAsync(); // LƯU NGAY để có ID
                        _logger.LogInformation("✅ Đã tạo và lưu khách hàng mới - ID: {Id}", khachHang.Id);
                    }
                    else
                    {
                        khachHang.HoTen = HoTen;
                        if (!string.IsNullOrWhiteSpace(Email)) khachHang.Email = Email;
                        if (!string.IsNullOrWhiteSpace(DiaChi)) khachHang.DiaChi = DiaChi;
                        _context.Update(khachHang);
                        await _context.SaveChangesAsync(); // LƯU NGAY để cập nhật
                        _logger.LogInformation("✅ Đã cập nhật khách hàng - ID: {Id}", khachHang.Id);
                    }

                    // 2. Tạo thú cưng - BÂY GIỜ khachHang.Id đã có giá trị
                    _logger.LogInformation("Bước 2: Tạo thú cưng cho khách hàng ID: {KhachHangId}", khachHang.Id);
                    var thuCung = new ThuCung
                    {
                        TenThuCung = TenThuCung,
                        Loai = Loai,
                        Giong = Giong,
                        Tuoi = Tuoi,
                        CanNang = CanNang,
                        GhiChuSucKhoe = GhiChuSucKhoe,
                        KhachHangId = khachHang.Id, // BÂY GIỜ ID đã có giá trị
                        NgayTao = DateTime.Now
                    };
                    _context.ThuCungs.Add(thuCung);
                    await _context.SaveChangesAsync(); // LƯU NGAY để có ID
                    _logger.LogInformation("✅ Đã tạo và lưu thú cưng - ID: {Id}, KhachHangId: {KhachHangId}", thuCung.Id, khachHang.Id);

                    // 3. Tạo đặt lịch - QUAN TRỌNG: DatLich LUÔN được tạo (cho cả Grooming và Hotel)
                    _logger.LogInformation("Bước 3: Tạo đặt lịch");
                    _logger.LogInformation("  - ThuCungId: {ThuCungId}", thuCung.Id);
                    _logger.LogInformation("  - PhongKhachSanId: {PhongId}", PhongKhachSanId);
                    _logger.LogInformation("  - DichVuGroomingIds: {GroomingIds}", 
                        DichVuGroomingIds != null ? string.Join(",", DichVuGroomingIds) : "null");
                    
                    datLich = new DatLich
                    {
                        ThoiGianHen = thoiGianHenParsed,
                        ThoiGianKetThuc = thoiGianKetThucParsed,
                        GhiChu = GhiChu,
                        GhiChuDacBiet = GhiChuDacBiet,
                        ThuCungId = thuCung.Id, // BÂY GIỜ ID đã có giá trị
                        PhongKhachSanId = PhongKhachSanId, // Optional - chỉ có khi đặt khách sạn
                        NgayTao = DateTime.Now,
                        TrangThai = "Chờ"
                    };
                    _context.DatLiches.Add(datLich);
                    await _context.SaveChangesAsync(); // LƯU NGAY để có ID
                    datLichId = datLich.Id;
                    _logger.LogInformation("✅ Đã tạo và lưu DatLich - ID: {Id}, ThuCungId: {ThuCungId}, PhongKhachSanId: {PhongId}", 
                        datLichId, thuCung.Id, PhongKhachSanId);

                    // 4. Lưu dịch vụ Grooming nếu có (SAU KHI DATLICH ĐÃ CÓ ID)
                    if (DichVuGroomingIds != null && DichVuGroomingIds.Length > 0)
                    {
                        _logger.LogInformation("Bước 4: Đang lưu {Count} dịch vụ Grooming cho DatLich ID: {DatLichId}", DichVuGroomingIds.Length, datLichId);
                        
                        // Verify DatLichId trước khi tạo DatLichDichVuGrooming
                        if (datLichId <= 0)
                        {
                            _logger.LogError("❌ LỖI NGHIÊM TRỌNG: DatLichId = {Id} không hợp lệ!", datLichId);
                            throw new Exception($"DatLichId không hợp lệ: {datLichId}");
                        }
                        
                        foreach (var groomingId in DichVuGroomingIds)
                        {
                            var datLichGrooming = new DatLichDichVuGrooming
                            {
                                DatLichId = datLichId,
                                DichVuGroomingId = groomingId
                            };
                            _context.DatLichDichVuGroomings.Add(datLichGrooming);
                            _logger.LogInformation("  - Đã thêm DatLichDichVuGrooming vào context: DatLichId={DatLichId}, DichVuGroomingId={GroomingId}", 
                                datLichId, groomingId);
                        }
                        
                        // Lưu dịch vụ Grooming
                        try
                        {
                            await _context.SaveChangesAsync();
                            _logger.LogInformation("✅ Đã lưu {Count} dịch vụ Grooming thành công cho DatLich ID: {Id}", 
                                DichVuGroomingIds.Length, datLichId);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "❌ LỖI KHI LƯU DỊCH VỤ GROOMING - DatLichId: {DatLichId}", datLichId);
                            throw;
                        }
                    }
                    else
                    {
                        _logger.LogInformation("Không có dịch vụ Grooming nào được chọn");
                    }

                    // Commit transaction
                    _logger.LogInformation("=== COMMIT TRANSACTION ===");
                    _logger.LogInformation("DatLich ID trước khi commit: {Id}", datLichId);
                    
                    if (datLichId <= 0)
                    {
                        _logger.LogError("❌ LỖI NGHIÊM TRỌNG: DatLichId = {Id} không hợp lệ trước khi commit!", datLichId);
                        throw new Exception($"DatLichId không hợp lệ: {datLichId}");
                    }
                    
                    await transaction.CommitAsync();
                    _logger.LogInformation("✅ TRANSACTION COMMITTED - Tất cả dữ liệu đã được lưu thành công!");
                    _logger.LogInformation("DatLich ID sau khi commit: {Id}", datLichId);
                    _logger.LogInformation("   - Khách hàng: {HoTen} ({SoDienThoai})", HoTen, SoDienThoai);
                    _logger.LogInformation("   - Thú cưng: {TenThuCung} ({Loai})", TenThuCung, Loai);
                    _logger.LogInformation("   - Thời gian hẹn: {ThoiGianHen}", thoiGianHenParsed);
                    _logger.LogInformation("   - Dịch vụ Grooming: {GroomingCount}, PhongKhachSanId: {PhongId}", 
                        DichVuGroomingIds?.Length ?? 0, PhongKhachSanId);
                    
                    // Đếm tổng số đặt lịch trong database sau khi commit
                    var totalAfterCommit = await _context.DatLiches.CountAsync();
                    _logger.LogInformation("   📊 Tổng số đặt lịch trong database sau commit: {TotalCount}", totalAfterCommit);

                    // Verify dữ liệu đã được lưu - Query lại từ database (không dùng cache)
                    // Đợi một chút để đảm bảo transaction đã commit hoàn toàn
                    await Task.Delay(100);
                    // KHÔNG dùng ChangeTracker.Clear() ở đây vì đã commit transaction rồi
                    // Chỉ cần dùng AsNoTracking() là đủ
                    
                    _logger.LogInformation("=== BẮT ĐẦU VERIFY DỮ LIỆU ===");
                    
                    // Đợi thêm một chút để đảm bảo transaction đã commit hoàn toàn
                    await Task.Delay(200);
                    
                    // Clear change tracker để query lại từ database
                    _context.ChangeTracker.Clear();
                    
                    var verify = await _context.DatLiches
                        .AsNoTracking()
                        .Include(d => d.ThuCung)
                            .ThenInclude(t => t != null ? t.KhachHang : null!)
                        .Include(d => d.DatLichDichVuGroomings)
                            .ThenInclude((DatLichDichVuGrooming dlg) => dlg.DichVuGrooming != null ? dlg.DichVuGrooming : null!)
                        .Include(d => d.PhongKhachSan)
                        .FirstOrDefaultAsync(d => d.Id == datLichId);

                    if (verify != null)
                    {
                        _logger.LogInformation("✅ VERIFY THÀNH CÔNG: DatLich ID {Id} đã có trong database!", verify.Id);
                        var customerName = verify.ThuCung?.KhachHang?.HoTen ?? "N/A";
                        var petName = verify.ThuCung?.TenThuCung ?? "N/A";
                        var groomingCount = verify.DatLichDichVuGroomings?.Count ?? 0;
                        _logger.LogInformation("   - Khách hàng: {HoTen}, Thú cưng: {TenThuCung}", 
                            customerName, petName);
                        _logger.LogInformation("   - Dịch vụ Grooming: {Count}, PhongKhachSanId: {PhongId}", 
                            groomingCount, verify.PhongKhachSanId);
                        
                        // Đếm tổng số đặt lịch trong database
                        var totalCount = await _context.DatLiches.CountAsync();
                        _logger.LogInformation("   📊 Tổng số đặt lịch trong database: {TotalCount}", totalCount);
                        
                        // Log để admin có thể thấy trong Debug Output
                        System.Diagnostics.Debug.WriteLine($"=== KHÁCH HÀNG ĐÃ ĐẶT LỊCH ===");
                        System.Diagnostics.Debug.WriteLine($"DatLich ID: {datLichId}");
                        System.Diagnostics.Debug.WriteLine($"Khách hàng: {customerName}");
                        System.Diagnostics.Debug.WriteLine($"Thú cưng: {petName}");
                        System.Diagnostics.Debug.WriteLine($"Thời gian: {verify.ThoiGianHen:dd/MM/yyyy HH:mm}");
                        System.Diagnostics.Debug.WriteLine($"Trạng thái: {verify.TrangThai}");
                        System.Diagnostics.Debug.WriteLine($"Tổng số đặt lịch trong DB: {totalCount}");

                        // Tạo thông báo cho admin - Sử dụng verify đã có đầy đủ thông tin
                        try
                        {
                            _logger.LogInformation("=== BẮT ĐẦU TẠO THÔNG BÁO ===");
                            _logger.LogInformation("DatLich ID: {Id}, ThuCungId: {ThuCungId}, KhachHang: {KhachHang}", 
                                verify.Id, verify.ThuCungId, verify.ThuCung?.KhachHang?.HoTen ?? "N/A");
                            
                            // Kiểm tra NotificationService có null không
                            if (_notificationService == null)
                            {
                                _logger.LogError("❌ NotificationService là NULL!");
                                System.Diagnostics.Debug.WriteLine("❌ NotificationService là NULL!");
                            }
                            else
                            {
                                _logger.LogInformation("✅ NotificationService đã được inject");
                                
                                // Gọi method tạo thông báo
                                await _notificationService.TaoThongBaoDatLichMoiAsync(verify);
                                
                                _logger.LogInformation("✅ Đã gọi TaoThongBaoDatLichMoiAsync thành công");
                                
                                // Kiểm tra xem có thông báo mới được tạo không
                                await Task.Delay(500); // Đợi một chút để thông báo được lưu
                                var thongBaoMoi = await _context.ThongBaos
                                    .Where(t => t.DatLichId == verify.Id && t.LoaiThongBao == LoaiThongBao.ADMIN_BOOKING_NEW)
                                    .OrderByDescending(t => t.ThoiGianTao)
                                    .FirstOrDefaultAsync();
                                
                                if (thongBaoMoi != null)
                                {
                                    _logger.LogInformation("✅ Đã tạo thông báo trong database - ID: {ThongBaoId}, Tiêu đề: {TieuDe}", 
                                        thongBaoMoi.Id, thongBaoMoi.TieuDe);
                                    System.Diagnostics.Debug.WriteLine($"✅ Thông báo đã được tạo - ID: {thongBaoMoi.Id}");
                                }
                                else
                                {
                                    _logger.LogWarning("⚠️ Không tìm thấy thông báo trong database sau khi gọi method");
                                    System.Diagnostics.Debug.WriteLine("⚠️ Không tìm thấy thông báo trong database");
                                }
                            }
                        }
                        catch (Exception notifEx)
                        {
                            _logger.LogError(notifEx, "❌ LỖI KHI TẠO THÔNG BÁO");
                            _logger.LogError("Exception Type: {Type}", notifEx.GetType().Name);
                            _logger.LogError("Exception Message: {Message}", notifEx.Message);
                            _logger.LogError("Exception StackTrace: {StackTrace}", notifEx.StackTrace);
                            if (notifEx.InnerException != null)
                            {
                                _logger.LogError("Inner Exception: {Message}", notifEx.InnerException.Message);
                            }
                            System.Diagnostics.Debug.WriteLine($"❌ Lỗi khi tạo thông báo: {notifEx.Message}");
                            System.Diagnostics.Debug.WriteLine($"StackTrace: {notifEx.StackTrace}");
                        }
                    }
                    else
                    {
                        _logger.LogError("❌ VERIFY THẤT BẠI: Không tìm thấy DatLich ID {Id} sau khi commit!", datLichId);
                        System.Diagnostics.Debug.WriteLine($"❌ VERIFY THẤT BẠI: Không tìm thấy DatLich ID {datLichId} sau khi commit!");
                        
                        // Thử query lại tất cả để xem có bao nhiêu đặt lịch
                        var allDatLich = await _context.DatLiches.CountAsync();
                        _logger.LogError("   Tổng số đặt lịch trong database: {TotalCount}", allDatLich);
                        System.Diagnostics.Debug.WriteLine($"   Tổng số đặt lịch trong database: {allDatLich}");
                    }

                TempData["Success"] = "Đặt lịch thành công! Chúng tôi sẽ liên hệ với bạn sớm nhất.";
                return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ LỖI TRONG TRANSACTION - BẮT ĐẦU ROLLBACK");
                    _logger.LogError("Exception Type: {Type}", ex.GetType().Name);
                    _logger.LogError("Exception Message: {Message}", ex.Message);
                    _logger.LogError("Stack Trace: {StackTrace}", ex.StackTrace);
                    
                    if (ex.InnerException != null)
                    {
                        _logger.LogError("Inner Exception Type: {Type}", ex.InnerException.GetType().Name);
                        _logger.LogError("Inner Exception Message: {Message}", ex.InnerException.Message);
                        _logger.LogError("Inner Stack Trace: {StackTrace}", ex.InnerException.StackTrace);
                    }
                    
                    // Log vào Debug Output
                    System.Diagnostics.Debug.WriteLine($"❌ LỖI TRONG TRANSACTION: {ex.GetType().Name}");
                    System.Diagnostics.Debug.WriteLine($"Message: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    }
                    
                    try
                    {
                        await transaction.RollbackAsync();
                        _logger.LogInformation("✅ Đã rollback transaction thành công");
                    }
                    catch (Exception rollbackEx)
                    {
                        _logger.LogError(rollbackEx, "❌ LỖI KHI ROLLBACK TRANSACTION");
                    }
                    
                    throw; // Re-throw để catch bên ngoài xử lý
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ LỖI KHI TẠO ĐẶT LỊCH - OUTER CATCH");
                _logger.LogError("Exception Type: {Type}", ex.GetType().Name);
                _logger.LogError("Exception Message: {Message}", ex.Message);
                _logger.LogError("Stack Trace: {StackTrace}", ex.StackTrace);
                
                if (ex.InnerException != null)
                {
                    _logger.LogError("Inner Exception Type: {Type}", ex.InnerException.GetType().Name);
                    _logger.LogError("Inner Exception Message: {Message}", ex.InnerException.Message);
                    _logger.LogError("Inner Stack Trace: {StackTrace}", ex.InnerException.StackTrace);
                }
                
                // Log vào Debug Output
                System.Diagnostics.Debug.WriteLine($"❌ LỖI KHI TẠO ĐẶT LỊCH: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"Message: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                
                TempData["Error"] = $"Có lỗi xảy ra khi đặt lịch: {ex.Message}. Vui lòng thử lại hoặc liên hệ admin!";
                return RedirectToAction("Index");
            }
        }
    }
}
