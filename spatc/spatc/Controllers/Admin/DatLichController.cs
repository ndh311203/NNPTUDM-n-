using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using spatc.Models;
using spatc.Services;

namespace spatc.Controllers.Admin
{
    [Authorize(Roles = "Admin,NhanVien")] // Chỉ Admin và Nhân viên mới truy cập được
    public class DatLichController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly NotificationService _notificationService;

        public DatLichController(ApplicationDbContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        // GET: Admin/DatLich - Quản lý đặt lịch (hiển thị TẤT CẢ đặt lịch từ khách hàng và admin)
        [HttpGet]
        [Route("/Admin/DatLich")]
        [Route("/Admin/DatLich/Index")]
        public async Task<IActionResult> Index(string? trangThai)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== Admin/DatLich/Index - BẮT ĐẦU QUERY ===");
                System.Diagnostics.Debug.WriteLine($"Filter trạng thái: {trangThai ?? "Tất cả"}");
                
                // Đếm tổng số đặt lịch TRƯỚC KHI query (để so sánh)
                var totalBeforeQuery = await _context.DatLiches.CountAsync();
                System.Diagnostics.Debug.WriteLine($"Tổng số đặt lịch trong DB (trước query): {totalBeforeQuery}");
                
                // Query TẤT CẢ đặt lịch (không filter theo nguồn)
                // QUAN TRỌNG: Query tất cả DatLich, bao gồm cả những DatLich có Grooming services
                // Đảm bảo Include đầy đủ để hiển thị tất cả thông tin
                IQueryable<DatLich> datLichQuery = _context.DatLiches
                    .Include(d => d.ThuCung)
                        .ThenInclude(t => t != null ? t.KhachHang : null!)
                    .Include(d => d.DichVu) // Dịch vụ cũ (nếu có)
                    .Include(d => d.DichVuGrooming) // Dịch vụ Grooming đơn (nếu có - không dùng nữa)
                    .Include(d => d.PhongKhachSan) // Phòng khách sạn
                    .Include(d => d.DatLichDichVuGroomings) // QUAN TRỌNG: Include để lấy nhiều dịch vụ Grooming
                        .ThenInclude((DatLichDichVuGrooming dlg) => dlg.DichVuGrooming != null ? dlg.DichVuGrooming : null!)
                    .Include(d => d.NhanVien) // Nhân viên
                    .AsNoTracking() // Không track để tránh cache
                    .OrderByDescending(d => d.NgayTao); // Sắp xếp theo ngày tạo (mới nhất trước)
                
                System.Diagnostics.Debug.WriteLine("✅ Query đã Include đầy đủ: ThuCung, KhachHang, DichVu, PhongKhachSan, DatLichDichVuGroomings, DichVuGrooming, NhanVien");

                // Filter theo trạng thái nếu có
                if (!string.IsNullOrEmpty(trangThai))
                {
                    datLichQuery = datLichQuery.Where(d => d.TrangThai == trangThai);
                    System.Diagnostics.Debug.WriteLine($"Đã áp dụng filter trạng thái: {trangThai}");
                }

                // Clear change tracker để đảm bảo query từ database (lấy dữ liệu mới nhất)
                _context.ChangeTracker.Clear();
                
                System.Diagnostics.Debug.WriteLine("Đang thực thi query...");
                var datLichList = await datLichQuery.ToListAsync();
                System.Diagnostics.Debug.WriteLine($"Query hoàn tất - Lấy được {datLichList.Count} đặt lịch");
                
                // Log để debug
                System.Diagnostics.Debug.WriteLine($"=== Admin/DatLich/Index - LOAD DỮ LIỆU ===");
                System.Diagnostics.Debug.WriteLine($"Tổng số đặt lịch query được: {datLichList.Count}");
                
                // Đếm trực tiếp từ database để so sánh
                var totalInDb = await _context.DatLiches.CountAsync();
                System.Diagnostics.Debug.WriteLine($"Tổng số đặt lịch trong database: {totalInDb}");
                
                if (datLichList.Count != totalInDb)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ CẢNH BÁO: Số lượng query ({datLichList.Count}) khác với tổng số trong DB ({totalInDb})");
                }
                
                // Log chi tiết từng đặt lịch (10 mới nhất)
                var recentOrders = datLichList.OrderByDescending(d => d.NgayTao).Take(10);
                System.Diagnostics.Debug.WriteLine("=== CHI TIẾT 10 ĐẶT LỊCH MỚI NHẤT ===");
                foreach (var dl in recentOrders)
                {
                    var customerName = dl.ThuCung?.KhachHang?.HoTen ?? "N/A";
                    var petName = dl.ThuCung?.TenThuCung ?? "N/A";
                    var serviceInfo = "";
                    
                    // Kiểm tra loại dịch vụ
                    if (dl.PhongKhachSan != null)
                    {
                        serviceInfo = $"🏨 {dl.PhongKhachSan.TenPhong}";
                    }
                    else if (dl.DatLichDichVuGroomings != null && dl.DatLichDichVuGroomings.Any())
                    {
                        var groomingNames = dl.DatLichDichVuGroomings
                            .Select(dlg => dlg.DichVuGrooming?.TenDichVu)
                            .Where(x => !string.IsNullOrEmpty(x))
                            .ToList();
                        serviceInfo = $"✂️ {string.Join(", ", groomingNames)} (Count: {dl.DatLichDichVuGroomings.Count})";
                    }
                    else if (dl.DichVu != null)
                    {
                        serviceInfo = $"📋 {dl.DichVu.TenDichVu}";
                    }
                    else
                    {
                        serviceInfo = "❌ Chưa có dịch vụ";
                    }
                    
                    System.Diagnostics.Debug.WriteLine($"  - ID: {dl.Id} | {dl.ThoiGianHen:dd/MM/yyyy HH:mm} | KH: {customerName} | TC: {petName} | DV: {serviceInfo} | TT: {dl.TrangThai}");
                    System.Diagnostics.Debug.WriteLine($"    ThuCungId: {dl.ThuCungId}, PhongKhachSanId: {dl.PhongKhachSanId}, GroomingCount: {dl.DatLichDichVuGroomings?.Count ?? 0}");
                }
                
                ViewBag.TrangThai = trangThai;
                ViewBag.TrangThaiList = new SelectList(new[] { "Chờ", "Đang làm", "Hoàn thành", "Hủy" }, trangThai);

                return View("~/Views/Admin/DatLich/Index.cshtml", datLichList);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải danh sách đặt lịch: {ex.Message}";
                return View("~/Views/Admin/DatLich/Index.cshtml", new List<DatLich>());
            }
        }

        // GET: Admin/DatLich/Create
        [HttpGet]
        [Route("/Admin/DatLich/Create")]
        public async Task<IActionResult> Create(int? dichVuId)
        {
            // Load dịch vụ Grooming và phòng khách sạn giống như trang khách hàng
            var groomingServices = await _context.DichVuGroomings
                .Where(d => d.TrangThai == true)
                .OrderBy(d => d.Gia)
                .ToListAsync();

            var phongKhachSans = await _context.PhongKhachSans
                .Where(p => p.TrangThai == "Trống")
                .OrderBy(p => p.GiaTheoNgay)
                .ToListAsync();

            // Nếu có dichVuId từ trang chủ, tìm dịch vụ Grooming tương ứng
            List<int> selectedGroomingIds = new List<int>();
            if (dichVuId.HasValue)
            {
                // Lấy thông tin dịch vụ từ trang chủ
                var dichVu = await _context.DichVus.FirstOrDefaultAsync(d => d.Id == dichVuId.Value && d.TrangThai == true);
                if (dichVu != null)
                {
                    // Tìm dịch vụ Grooming có tên tương tự hoặc giống hệt
                    var matchingGrooming = groomingServices
                        .FirstOrDefault(g => g.TenDichVu.ToLower().Trim() == dichVu.TenDichVu.ToLower().Trim());
                    
                    if (matchingGrooming != null)
                    {
                        selectedGroomingIds.Add(matchingGrooming.Id);
                    }
                    else
                    {
                        // Nếu không tìm thấy tên chính xác, thử tìm theo tên gần giống
                        var similarGrooming = groomingServices
                            .FirstOrDefault(g => g.TenDichVu.ToLower().Contains(dichVu.TenDichVu.ToLower()) || 
                                                 dichVu.TenDichVu.ToLower().Contains(g.TenDichVu.ToLower()));
                        if (similarGrooming != null)
                        {
                            selectedGroomingIds.Add(similarGrooming.Id);
                        }
                    }
                }
            }

            ViewBag.GroomingServices = groomingServices;
            ViewBag.PhongKhachSans = phongKhachSans;
            ViewBag.SelectedGroomingIds = selectedGroomingIds;
            ViewData["NhanVienId"] = new SelectList(await _context.NhanViens.Where(n => n.TrangThai == true).OrderBy(n => n.HoTen).ToListAsync(), "Id", "HoTen");
            ViewBag.TrangThaiList = new SelectList(new[] { "Chờ", "Đang làm", "Hoàn thành", "Hủy" }, "Chờ");
            
            return View("~/Views/Admin/DatLich/Create.cshtml");
        }

        // POST: Admin/DatLich/Create
        [HttpPost]
        [Route("/Admin/DatLich/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            int[]? DichVuGroomingIds, // Mảng các ID dịch vụ Grooming được chọn
            int? PhongKhachSanId,
            string khachHangHoTen,
            string khachHangSoDienThoai,
            string? khachHangEmail,
            string? khachHangDiaChi,
            string thuCungTen,
            string thuCungLoai,
            string? thuCungGiong,
            int? thuCungTuoi,
            decimal? thuCungCanNang,
            string? thuCungGhiChu,
            string ThoiGianHen,
            string? ThoiGianKetThuc,
            string? GhiChu,
            string? GhiChuDacBiet,
            int? NhanVienId,
            string? TrangThai)
        {
            // Parse thời gian hẹn (datetime-local format: yyyy-MM-ddTHH:mm)
            DateTime thoiGianHenParsed = DateTime.Now;
            bool isValidThoiGianHen = false;
            
            if (string.IsNullOrEmpty(ThoiGianHen) || !DateTime.TryParse(ThoiGianHen, out thoiGianHenParsed))
            {
                ModelState.AddModelError("", "Thời gian hẹn không hợp lệ!");
            }
            else
            {
                isValidThoiGianHen = true;
            }

            // Đảm bảo TrangThai có giá trị mặc định nếu null hoặc empty
            if (string.IsNullOrWhiteSpace(TrangThai))
            {
                TrangThai = "Chờ";
            }

            // Validate thông tin khách hàng
            if (string.IsNullOrWhiteSpace(khachHangHoTen) || string.IsNullOrWhiteSpace(khachHangSoDienThoai))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin khách hàng (Họ tên và Số điện thoại)");
            }

            // Validate thông tin thú cưng
            if (string.IsNullOrWhiteSpace(thuCungTen) || string.IsNullOrWhiteSpace(thuCungLoai))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin thú cưng (Tên và Loài)");
            }

            // Kiểm tra loại dịch vụ
            if ((DichVuGroomingIds == null || DichVuGroomingIds.Length == 0) && !PhongKhachSanId.HasValue)
            {
                ModelState.AddModelError("", "Vui lòng chọn ít nhất một dịch vụ Grooming hoặc phòng khách sạn!");
            }

            // Chỉ tiếp tục nếu tất cả validation đều pass
            if (ModelState.IsValid && isValidThoiGianHen)
            {
                // Sử dụng transaction để đảm bảo tất cả dữ liệu được lưu cùng lúc (giống như khách hàng)
                using var transaction = await _context.Database.BeginTransactionAsync();
                
                try
                {
                    System.Diagnostics.Debug.WriteLine("=== ADMIN TẠO ĐẶT LỊCH ===");
                    
                    // 1. Tìm hoặc tạo khách hàng
                    var khachHang = await _context.KhachHangs
                        .FirstOrDefaultAsync(k => k.SoDienThoai == khachHangSoDienThoai);

                    if (khachHang == null)
                    {
                        khachHang = new KhachHang
                        {
                            HoTen = khachHangHoTen,
                            SoDienThoai = khachHangSoDienThoai,
                            Email = khachHangEmail,
                            DiaChi = khachHangDiaChi,
                            NgayTao = DateTime.Now
                        };
                        _context.KhachHangs.Add(khachHang);
                        await _context.SaveChangesAsync();
                        System.Diagnostics.Debug.WriteLine($"Đã tạo khách hàng mới - ID: {khachHang.Id}");
                    }
                    else
                    {
                        khachHang.HoTen = khachHangHoTen;
                        if (!string.IsNullOrWhiteSpace(khachHangEmail))
                            khachHang.Email = khachHangEmail;
                        if (!string.IsNullOrWhiteSpace(khachHangDiaChi))
                            khachHang.DiaChi = khachHangDiaChi;
                        _context.Update(khachHang);
                        await _context.SaveChangesAsync();
                        System.Diagnostics.Debug.WriteLine($"Đã cập nhật khách hàng - ID: {khachHang.Id}");
                    }

                    // 2. Tạo thú cưng
                    var thuCung = new ThuCung
                    {
                        TenThuCung = thuCungTen,
                        Loai = thuCungLoai,
                        Giong = thuCungGiong,
                        Tuoi = thuCungTuoi,
                        CanNang = thuCungCanNang,
                        GhiChuSucKhoe = thuCungGhiChu,
                        KhachHangId = khachHang.Id,
                        NgayTao = DateTime.Now
                    };
                    _context.ThuCungs.Add(thuCung);
                    await _context.SaveChangesAsync();
                    System.Diagnostics.Debug.WriteLine($"Đã tạo thú cưng - ID: {thuCung.Id}");

                    // 3. Parse thời gian kết thúc (cho khách sạn)
                    DateTime? thoiGianKetThuc = null;
                    if (!string.IsNullOrEmpty(ThoiGianKetThuc) && DateTime.TryParse(ThoiGianKetThuc, out DateTime parsedEnd))
                    {
                        thoiGianKetThuc = parsedEnd;
                    }

                    // 4. Tạo đặt lịch
                    var datLich = new DatLich
                    {
                        ThoiGianHen = thoiGianHenParsed,
                        ThoiGianKetThuc = thoiGianKetThuc,
                        GhiChu = GhiChu,
                        GhiChuDacBiet = GhiChuDacBiet,
                        ThuCungId = thuCung.Id,
                        PhongKhachSanId = PhongKhachSanId,
                        NhanVienId = NhanVienId,
                        TrangThai = TrangThai,
                        NgayTao = DateTime.Now
                    };
                    
                    _context.Add(datLich);
                    await _context.SaveChangesAsync();
                    System.Diagnostics.Debug.WriteLine($"✅ Đã tạo DatLich - ID: {datLich.Id}");

                    // 5. Lưu nhiều dịch vụ Grooming nếu có
                    if (DichVuGroomingIds != null && DichVuGroomingIds.Length > 0)
                    {
                        foreach (var groomingId in DichVuGroomingIds)
                        {
                            var datLichGrooming = new DatLichDichVuGrooming
                            {
                                DatLichId = datLich.Id,
                                DichVuGroomingId = groomingId
                            };
                            _context.DatLichDichVuGroomings.Add(datLichGrooming);
                        }
                        await _context.SaveChangesAsync();
                        System.Diagnostics.Debug.WriteLine($"✅ Đã lưu {DichVuGroomingIds.Length} dịch vụ Grooming");
                    }

                    // Commit transaction
                    await transaction.CommitAsync();
                    System.Diagnostics.Debug.WriteLine("✅ TRANSACTION COMMITTED - Tất cả dữ liệu đã được lưu!");

                    // Verify dữ liệu đã được lưu
                    var verifyDatLich = await _context.DatLiches
                        .AsNoTracking()
                        .Include(d => d.ThuCung)
                        .ThenInclude(t => t.KhachHang)
                        .Include(d => d.DatLichDichVuGroomings)
                        .ThenInclude(dlg => dlg.DichVuGrooming)
                        .Include(d => d.PhongKhachSan)
                        .FirstOrDefaultAsync(d => d.Id == datLich.Id);
                    
                    if (verifyDatLich != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"✅ VERIFY THÀNH CÔNG: DatLich ID {verifyDatLich.Id} đã có trong database!");
                    }

                    TempData["Success"] = "Thêm đặt lịch thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    System.Diagnostics.Debug.WriteLine($"❌ Lỗi trong transaction, đã rollback: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                    ModelState.AddModelError("", $"Có lỗi xảy ra khi tạo đặt lịch: {ex.Message}");
                }
            }
            else
            {
                // Log các lỗi validation
                System.Diagnostics.Debug.WriteLine("=== Validation Errors ===");
                foreach (var error in ModelState)
                {
                    foreach (var err in error.Value.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"Key: {error.Key}, Error: {err.ErrorMessage}");
                    }
                }
            }

            // Reload dropdowns nếu có lỗi và giữ lại dữ liệu đã nhập
            var groomingServices = await _context.DichVuGroomings
                .Where(d => d.TrangThai == true)
                .OrderBy(d => d.Gia)
                .ToListAsync();

            var phongKhachSans = await _context.PhongKhachSans
                .Where(p => p.TrangThai == "Trống")
                .OrderBy(p => p.GiaTheoNgay)
                .ToListAsync();

            ViewBag.GroomingServices = groomingServices;
            ViewBag.PhongKhachSans = phongKhachSans;
            ViewBag.SelectedGroomingIds = DichVuGroomingIds != null ? DichVuGroomingIds.ToList() : new List<int>();
            ViewBag.SelectedPhongKhachSanId = PhongKhachSanId;
            ViewBag.KhachHangHoTen = khachHangHoTen;
            ViewBag.KhachHangSoDienThoai = khachHangSoDienThoai;
            ViewBag.KhachHangEmail = khachHangEmail;
            ViewBag.KhachHangDiaChi = khachHangDiaChi;
            ViewBag.ThuCungTen = thuCungTen;
            ViewBag.ThuCungLoai = thuCungLoai;
            ViewBag.ThuCungGiong = thuCungGiong;
            ViewBag.ThuCungTuoi = thuCungTuoi;
            ViewBag.ThuCungCanNang = thuCungCanNang;
            ViewBag.ThuCungGhiChu = thuCungGhiChu;
            ViewBag.ThoiGianHen = ThoiGianHen;
            ViewBag.ThoiGianKetThuc = ThoiGianKetThuc;
            ViewBag.GhiChu = GhiChu;
            ViewBag.GhiChuDacBiet = GhiChuDacBiet;
            ViewBag.NhanVienId = NhanVienId;
            ViewBag.TrangThai = TrangThai;
            ViewData["NhanVienId"] = new SelectList(await _context.NhanViens.Where(n => n.TrangThai == true).OrderBy(n => n.HoTen).ToListAsync(), "Id", "HoTen", NhanVienId);
            ViewBag.TrangThaiList = new SelectList(new[] { "Chờ", "Đang làm", "Hoàn thành", "Hủy" }, TrangThai);
            
            return View("~/Views/Admin/DatLich/Create.cshtml");
        }

        // GET: API/DatLich/GetKhachHangInfo
        [HttpGet]
        [Route("/Admin/DatLich/GetKhachHangInfo/{khachHangId:int}")]
        public async Task<IActionResult> GetKhachHangInfo(int khachHangId)
        {
            var khachHang = await _context.KhachHangs
                .Include(k => k.ThuCungs)
                .FirstOrDefaultAsync(k => k.Id == khachHangId);
            
            if (khachHang == null)
            {
                return Json(new { success = false });
            }

            var soLuongThuCung = khachHang.ThuCungs?.Count ?? 0;
            
            return Json(new 
            { 
                success = true,
                id = khachHang.Id,
                hoTen = khachHang.HoTen,
                soDienThoai = khachHang.SoDienThoai,
                email = khachHang.Email,
                diaChi = khachHang.DiaChi,
                soLuongThuCung = soLuongThuCung,
                ghiChu = khachHang.GhiChu
            });
        }

        // GET: API/DatLich/GetThuCungsByKhachHang
        [HttpGet]
        [Route("/Admin/DatLich/GetThuCungsByKhachHang/{khachHangId:int}")]
        public async Task<IActionResult> GetThuCungsByKhachHang(int khachHangId)
        {
            var thuCungs = await _context.ThuCungs
                .Where(t => t.KhachHangId == khachHangId)
                .Select(t => new 
                { 
                    id = t.Id, 
                    tenThuCung = t.TenThuCung, 
                    loai = t.Loai, 
                    giong = t.Giong ?? "",
                    tuoi = t.Tuoi,
                    canNang = t.CanNang,
                    ghiChuSucKhoe = t.GhiChuSucKhoe ?? ""
                })
                .ToListAsync();
            
            return Json(thuCungs);
        }

        // GET: API/DatLich/GetThuCungInfo
        [HttpGet]
        [Route("/Admin/DatLich/GetThuCungInfo/{thuCungId:int}")]
        public async Task<IActionResult> GetThuCungInfo(int thuCungId)
        {
            var thuCung = await _context.ThuCungs
                .FirstOrDefaultAsync(t => t.Id == thuCungId);
            
            if (thuCung == null)
            {
                return Json(new { success = false });
            }

            return Json(new 
            { 
                success = true,
                id = thuCung.Id,
                tenThuCung = thuCung.TenThuCung,
                loai = thuCung.Loai,
                giong = thuCung.Giong ?? "",
                tuoi = thuCung.Tuoi,
                canNang = thuCung.CanNang,
                ghiChuSucKhoe = thuCung.GhiChuSucKhoe ?? "",
                hinhAnh = thuCung.HinhAnh ?? ""
            });
        }

        // POST: API/DatLich/CreateKhachHangQuick
        [HttpPost]
        [Route("/Admin/DatLich/CreateKhachHangQuick")]
        public async Task<IActionResult> CreateKhachHangQuick([FromBody] KhachHang khachHang)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra số điện thoại đã tồn tại chưa
                var existing = await _context.KhachHangs.FirstOrDefaultAsync(k => k.SoDienThoai == khachHang.SoDienThoai);
                if (existing != null)
                {
                    return Json(new { success = false, message = "Số điện thoại đã tồn tại!", id = existing.Id, hoTen = existing.HoTen });
                }

                khachHang.NgayTao = DateTime.Now;
                _context.KhachHangs.Add(khachHang);
                await _context.SaveChangesAsync();

                return Json(new { success = true, id = khachHang.Id, hoTen = khachHang.HoTen, soDienThoai = khachHang.SoDienThoai });
            }

            return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
        }

        // POST: API/DatLich/CreateThuCungQuick
        [HttpPost]
        [Route("/Admin/DatLich/CreateThuCungQuick")]
        public async Task<IActionResult> CreateThuCungQuick([FromBody] ThuCung thuCung)
        {
            // Bỏ qua validation cho KhachHangId vì sẽ set sau
            ModelState.Remove("KhachHangId");
            
            if (ModelState.IsValid && thuCung.KhachHangId > 0)
            {
                thuCung.NgayTao = DateTime.Now;
                _context.ThuCungs.Add(thuCung);
                await _context.SaveChangesAsync();

                return Json(new { success = true, id = thuCung.Id, tenThuCung = thuCung.TenThuCung, loai = thuCung.Loai, giong = thuCung.Giong });
            }

            return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
        }

        // GET: Admin/DatLich/Details/5
        [HttpGet]
        [Route("/Admin/DatLich/Details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var datLich = await _context.DatLiches
                .Include(d => d.ThuCung)
                .ThenInclude(t => t.KhachHang)
                .Include(d => d.DichVu)
                .Include(d => d.NhanVien)
                .Include(d => d.HoaDon)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (datLich == null)
            {
                return NotFound();
            }

            ViewData["NhanVienId"] = new SelectList(_context.NhanViens.Where(n => n.TrangThai == true), "Id", "HoTen", datLich.NhanVienId);
            return View("~/Views/Admin/DatLich/Details.cshtml", datLich);
        }

        // POST: Admin/DatLich/UpdateStatus
        [HttpPost]
        [Route("/Admin/DatLich/UpdateStatus")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string trangThai, int? nhanVienId, string? ghiChu)
        {
            var datLich = await _context.DatLiches
                .Include(d => d.ThuCung)
                .FirstOrDefaultAsync(d => d.Id == id);
            if (datLich == null)
            {
                return NotFound();
            }

            datLich.TrangThai = trangThai;
            if (nhanVienId.HasValue)
            {
                datLich.NhanVienId = nhanVienId;
            }
            if (!string.IsNullOrEmpty(ghiChu))
            {
                datLich.GhiChu = ghiChu;
            }

            _context.Update(datLich);
            await _context.SaveChangesAsync();

            // Gửi thông báo nếu trạng thái thay đổi thành các trạng thái đặc biệt
            if (datLich.ThuCungId > 0 && 
                (trangThai == "Đang làm" || trangThai == "Hoàn thành" || trangThai == "Hủy"))
            {
                try
                {
                    string trangThaiThuCung = trangThai switch
                    {
                        "Đang làm" => "BATHING",
                        "Hoàn thành" => "DONE",
                        "Hủy" => "ISSUE",
                        _ => ""
                    };

                    if (!string.IsNullOrEmpty(trangThaiThuCung))
                    {
                        await _notificationService.TaoThongBaoTrangThaiThuCungAsync(
                            datLich.ThuCungId,
                            trangThaiThuCung,
                            ghiChu
                        );
                    }
                }
                catch (Exception ex)
                {
                    // Log lỗi nhưng không làm gián đoạn flow
                    System.Diagnostics.Debug.WriteLine($"Lỗi khi gửi thông báo: {ex.Message}");
                }
            }

            TempData["Success"] = "Cập nhật trạng thái thành công!";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Admin/DatLich/UpdatePetStatus
        // API để admin cập nhật trạng thái thú cưng trong quá trình dịch vụ
        [HttpPost]
        [Route("/Admin/DatLich/UpdatePetStatus")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UpdatePetStatus([FromBody] UpdatePetStatusRequest request)
        {
            try
            {
                if (request == null || request.ThuCungId <= 0 || string.IsNullOrEmpty(request.TrangThai))
                {
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
                }

                var thuCung = await _context.ThuCungs.FindAsync(request.ThuCungId);
                if (thuCung == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy thú cưng" });
                }

                // Tạo thông báo cho khách hàng
                await _notificationService.TaoThongBaoTrangThaiThuCungAsync(
                    request.ThuCungId,
                    request.TrangThai,
                    request.MoTaChiTiet
                );

                return Json(new { success = true, message = "Đã cập nhật trạng thái và gửi thông báo" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        public class UpdatePetStatusRequest
        {
            public int ThuCungId { get; set; }
            public string TrangThai { get; set; } = string.Empty; // CHECKIN, BATHING, DRYING, TRIMMING, DONE, HOTEL_DAILY, ISSUE
            public string? MoTaChiTiet { get; set; }
        }

        // GET: Admin/DatLich/Delete/5
        [HttpGet]
        [Route("/Admin/DatLich/Delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var datLich = await _context.DatLiches
                .Include(d => d.ThuCung)
                .Include(d => d.DichVu)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (datLich == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/DatLich/Delete.cshtml", datLich);
        }

        // POST: Admin/DatLich/Delete/5
        [HttpPost]
        [Route("/Admin/DatLich/Delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var datLich = await _context.DatLiches.FindAsync(id);
            if (datLich != null)
            {
                _context.DatLiches.Remove(datLich);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa đặt lịch thành công!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

