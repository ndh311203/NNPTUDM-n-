using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using spatc.Models;

namespace spatc.Controllers
{
    public class RegisterController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RegisterController> _logger;

        public RegisterController(ApplicationDbContext context, ILogger<RegisterController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Register
        public IActionResult Index()
        {
            // Nếu đã đăng nhập, chuyển về trang chủ
            if (User.Identity?.IsAuthenticated == true)
            {
                var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                if (userRole == "Admin" || userRole == "NhanVien")
                {
                    return RedirectToAction("Index", "Admin");
                }
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string email, string matKhau, string hoTen, string xacNhanMatKhau, string soDienThoai)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(matKhau))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin!";
                return View();
            }

            if (string.IsNullOrEmpty(hoTen))
            {
                ViewBag.Error = "Vui lòng nhập họ tên!";
                return View();
            }

            if (string.IsNullOrEmpty(soDienThoai))
            {
                ViewBag.Error = "Vui lòng nhập số điện thoại!";
                return View();
            }

            if (matKhau != xacNhanMatKhau)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp!";
                return View();
            }

            if (matKhau.Length < 6)
            {
                ViewBag.Error = "Mật khẩu phải có ít nhất 6 ký tự!";
                return View();
            }

            try
            {
                // Kiểm tra email đã tồn tại chưa
                var existingAccount = await _context.TaiKhoans
                    .FirstOrDefaultAsync(t => t.Email == email);

                if (existingAccount != null)
                {
                    ViewBag.Error = "Email này đã được sử dụng!";
                    return View();
                }

                // Kiểm tra số điện thoại đã tồn tại chưa
                var existingKhachHang = await _context.KhachHangs
                    .FirstOrDefaultAsync(k => k.SoDienThoai == soDienThoai);

                if (existingKhachHang != null)
                {
                    ViewBag.Error = "Số điện thoại này đã được sử dụng!";
                    return View();
                }

                // Hash mật khẩu
                var hashedPassword = HashPassword(matKhau);

                // Tạo tài khoản mới (tài khoản bình thường)
                var taiKhoan = new TaiKhoan
                {
                    Email = email,
                    MatKhau = hashedPassword,
                    HoTen = hoTen,
                    VaiTro = "User", // Tài khoản bình thường
                    TrangThai = true,
                    NgayTao = DateTime.Now,
                    Provider = null, // Đăng ký local
                    ProviderId = null
                };

                _context.TaiKhoans.Add(taiKhoan);
                await _context.SaveChangesAsync();

                // Tạo khách hàng liên kết với tài khoản
                var khachHang = new KhachHang
                {
                    HoTen = hoTen,
                    SoDienThoai = soDienThoai,
                    Email = email,
                    TaiKhoanId = taiKhoan.Id,
                    NgayTao = DateTime.Now
                };

                _context.KhachHangs.Add(khachHang);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Tài khoản mới đã được đăng ký: {email} với khách hàng ID: {khachHang.Id}");

                TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đăng ký");
                ViewBag.Error = "Đã xảy ra lỗi khi đăng ký. Vui lòng thử lại!";
                return View();
            }
        }

        // Hàm hash mật khẩu (SHA256)
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}

