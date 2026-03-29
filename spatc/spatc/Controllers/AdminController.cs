using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spatc.Models;

namespace spatc.Controllers
{
    [Authorize(Roles = "Admin,NhanVien")] // Chỉ Admin và Nhân viên mới truy cập được
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ApplicationDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            ViewBag.UserName = User.Identity?.Name;
            ViewBag.UserEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            ViewBag.UserRole = userRole;

            // Thống kê
            ViewBag.TongDatLich = await _context.DatLiches.CountAsync();
            ViewBag.DatLichCho = await _context.DatLiches.CountAsync(d => d.TrangThai == "Chờ");
            ViewBag.DatLichDangLam = await _context.DatLiches.CountAsync(d => d.TrangThai == "Đang làm");
            ViewBag.DatLichHoanThanh = await _context.DatLiches.CountAsync(d => d.TrangThai == "Hoàn thành");
            ViewBag.TongKhachHang = await _context.KhachHangs.CountAsync();
            ViewBag.TongThuCung = await _context.ThuCungs.CountAsync();
            ViewBag.TongDichVu = await _context.DichVus.CountAsync();
            ViewBag.TongHoaDon = await _context.HoaDons.CountAsync();
            ViewBag.TongDoanhThu = await _context.HoaDons.SumAsync(h => h.TongTien);

            // Đặt lịch mới nhất
            ViewBag.DatLichMoi = await _context.DatLiches
                .Include(d => d.ThuCung)
                .ThenInclude(t => t.KhachHang)
                .Include(d => d.DichVu)
                .OrderByDescending(d => d.NgayTao)
                .Take(5)
                .ToListAsync();
            
            return View();
        }
    }
}

