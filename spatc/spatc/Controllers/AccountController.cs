using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using spatc.Models;

namespace spatc.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ApplicationDbContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Account/Profile
        [HttpGet]
        [Route("Account/Profile")]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int accountId))
            {
                return RedirectToAction("Index", "Login");
            }

            var taiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(t => t.Id == accountId);

            if (taiKhoan == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Tìm hoặc tạo khách hàng liên kết
            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(k => k.TaiKhoanId == accountId);

            if (khachHang == null)
            {
                // Tạo khách hàng mới nếu chưa có
                khachHang = new KhachHang
                {
                    HoTen = taiKhoan.HoTen ?? "",
                    Email = taiKhoan.Email,
                    TaiKhoanId = accountId,
                    NgayTao = DateTime.Now
                };
                _context.KhachHangs.Add(khachHang);
                await _context.SaveChangesAsync();
            }

            ViewBag.KhachHang = khachHang;
            return View("~/Views/Account/Profile.cshtml", taiKhoan);
        }

        // POST: Account/UpdateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Account/UpdateProfile")]
        public async Task<IActionResult> UpdateProfile(string hoTen, string soDienThoai, string email, string diaChi)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int accountId))
            {
                return RedirectToAction("Index", "Login");
            }

            var taiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(t => t.Id == accountId);

            if (taiKhoan == null)
            {
                TempData["Error"] = "Không tìm thấy tài khoản!";
                return RedirectToAction("Profile");
            }

            try
            {
                // Cập nhật thông tin tài khoản
                if (!string.IsNullOrEmpty(hoTen))
                {
                    taiKhoan.HoTen = hoTen;
                }

                // Tìm hoặc tạo khách hàng
                var khachHang = await _context.KhachHangs
                    .FirstOrDefaultAsync(k => k.TaiKhoanId == accountId);

                if (khachHang == null)
                {
                    khachHang = new KhachHang
                    {
                        TaiKhoanId = accountId,
                        NgayTao = DateTime.Now
                    };
                    _context.KhachHangs.Add(khachHang);
                }

                khachHang.HoTen = hoTen ?? taiKhoan.HoTen ?? "";
                khachHang.SoDienThoai = soDienThoai ?? khachHang.SoDienThoai ?? "";
                khachHang.Email = email ?? taiKhoan.Email ?? "";
                khachHang.DiaChi = diaChi;

                await _context.SaveChangesAsync();

                // Cập nhật claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, taiKhoan.Id.ToString()),
                    new Claim(ClaimTypes.Email, taiKhoan.Email),
                    new Claim(ClaimTypes.Name, taiKhoan.HoTen ?? taiKhoan.Email),
                    new Claim(ClaimTypes.Role, taiKhoan.VaiTro)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                };
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                TempData["Success"] = "Cập nhật thông tin thành công!";
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật profile");
                TempData["Error"] = "Đã xảy ra lỗi khi cập nhật thông tin!";
                return RedirectToAction("Profile");
            }
        }

        // GET: Account/ChangePassword
        [HttpGet]
        [Route("Account/ChangePassword")]
        public IActionResult ChangePassword()
        {
            return View("~/Views/Account/ChangePassword.cshtml");
        }

        // POST: Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Account/ChangePassword")]
        public async Task<IActionResult> ChangePassword(string matKhauCu, string matKhauMoi, string xacNhanMatKhauMoi)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int accountId))
            {
                return RedirectToAction("Index", "Login");
            }

            if (string.IsNullOrEmpty(matKhauCu) || string.IsNullOrEmpty(matKhauMoi))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin!";
                return View("~/Views/Account/ChangePassword.cshtml");
            }

            if (matKhauMoi != xacNhanMatKhauMoi)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp!";
                return View("~/Views/Account/ChangePassword.cshtml");
            }

            if (matKhauMoi.Length < 6)
            {
                ViewBag.Error = "Mật khẩu mới phải có ít nhất 6 ký tự!";
                return View("~/Views/Account/ChangePassword.cshtml");
            }

            var taiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(t => t.Id == accountId);

            if (taiKhoan == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Kiểm tra mật khẩu cũ
            var hashedOldPassword = HashPassword(matKhauCu);
            if (taiKhoan.MatKhau != hashedOldPassword)
            {
                ViewBag.Error = "Mật khẩu cũ không đúng!";
                return View("~/Views/Account/ChangePassword.cshtml");
            }

            try
            {
                // Cập nhật mật khẩu mới
                taiKhoan.MatKhau = HashPassword(matKhauMoi);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Đổi mật khẩu thành công!";
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đổi mật khẩu");
                ViewBag.Error = "Đã xảy ra lỗi khi đổi mật khẩu!";
                return View("~/Views/Account/ChangePassword.cshtml");
            }
        }

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

