using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using spatc.Models;

namespace spatc.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LoginController> _logger;
        private readonly IConfiguration _configuration;

        public LoginController(ApplicationDbContext context, ILogger<LoginController> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        // GET: Login
        public IActionResult Index()
        {
            // Nếu đã đăng nhập, chuyển hướng theo role
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

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string email, string matKhau)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(matKhau))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin!";
                return View();
            }

            try
            {
                // Hash mật khẩu để so sánh
                var hashedPassword = HashPassword(matKhau);

                // Tìm tài khoản
                var taiKhoan = await _context.TaiKhoans
                    .FirstOrDefaultAsync(t => t.Email == email && t.MatKhau == hashedPassword && t.TrangThai == true);

                if (taiKhoan == null)
                {
                    ViewBag.Error = "Email hoặc mật khẩu không đúng!";
                    return View();
                }

                // Tạo claims
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
                    IsPersistent = true, // Lưu cookie
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7) // Cookie tồn tại 7 ngày
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                _logger.LogInformation($"User {email} đã đăng nhập thành công.");

                // Chuyển hướng theo vai trò
                if (taiKhoan.VaiTro == "Admin" || taiKhoan.VaiTro == "NhanVien")
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    // Tài khoản bình thường chuyển về trang chủ
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đăng nhập");
                ViewBag.Error = "Đã xảy ra lỗi khi đăng nhập. Vui lòng thử lại!";
                return View();
            }
        }

        // GET: Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // GET: Google Login
        public IActionResult GoogleLogin()
        {
            var clientId = _configuration["Authentication:Google:ClientId"];
            if (string.IsNullOrEmpty(clientId) || clientId == "YOUR_GOOGLE_CLIENT_ID")
            {
                TempData["Error"] = "Google OAuth chưa được cấu hình! Vui lòng cập nhật ClientId và ClientSecret trong appsettings.json. Xem file OAUTH_SETUP_GUIDE.md để biết chi tiết.";
                return RedirectToAction("Index");
            }

            var redirectUrl = Url.Action("GoogleCallback", "Login");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, "Google");
        }

        // GET: Facebook Login
        public IActionResult FacebookLogin()
        {
            var appId = _configuration["Authentication:Facebook:AppId"];
            if (string.IsNullOrEmpty(appId) || appId == "YOUR_FACEBOOK_APP_ID")
            {
                TempData["Error"] = "Facebook OAuth chưa được cấu hình! Vui lòng cập nhật AppId và AppSecret trong appsettings.json. Xem file OAUTH_SETUP_GUIDE.md để biết chi tiết.";
                return RedirectToAction("Index");
            }

            var redirectUrl = Url.Action("FacebookCallback", "Login");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, "Facebook");
        }

        // GET: Google Callback
        public async Task<IActionResult> GoogleCallback()
        {
            var result = await HttpContext.AuthenticateAsync("Google");
            if (!result.Succeeded)
            {
                _logger.LogWarning("Google authentication failed");
                TempData["Error"] = "Đăng nhập Google thất bại. Vui lòng thử lại!";
                return RedirectToAction("Index");
            }

            var claims = result.Principal.Claims.ToList();
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var providerId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Không thể lấy thông tin email từ Google!";
                return RedirectToAction("Index");
            }

            return await ProcessOAuthLogin(email, name, "Google", providerId);
        }

        // GET: Facebook Callback
        public async Task<IActionResult> FacebookCallback()
        {
            var result = await HttpContext.AuthenticateAsync("Facebook");
            if (!result.Succeeded)
            {
                _logger.LogWarning("Facebook authentication failed");
                TempData["Error"] = "Đăng nhập Facebook thất bại. Vui lòng thử lại!";
                return RedirectToAction("Index");
            }

            var claims = result.Principal.Claims.ToList();
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var providerId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            // Facebook có thể không trả về email nếu user chưa cấp quyền
            // Sử dụng providerId để tạo email tạm thời nếu không có email
            if (string.IsNullOrEmpty(email))
            {
                if (!string.IsNullOrEmpty(providerId))
                {
                    // Tạo email tạm thời từ providerId
                    email = $"facebook_{providerId}@oauth.temp";
                    _logger.LogWarning($"Facebook không trả về email, sử dụng email tạm: {email}");
                }
                else
                {
                    TempData["Error"] = "Không thể lấy thông tin từ Facebook! Vui lòng cấp quyền truy cập email.";
                    return RedirectToAction("Index");
                }
            }

            return await ProcessOAuthLogin(email, name, "Facebook", providerId);
        }

        // Xử lý đăng nhập OAuth
        private async Task<IActionResult> ProcessOAuthLogin(string email, string? name, string provider, string? providerId)
        {
            try
            {
                // Tìm tài khoản đã tồn tại - ưu tiên tìm theo providerId, sau đó mới tìm theo email
                TaiKhoan? taiKhoan = null;
                
                if (!string.IsNullOrEmpty(providerId))
                {
                    taiKhoan = await _context.TaiKhoans
                        .FirstOrDefaultAsync(t => t.Provider == provider && t.ProviderId == providerId);
                }

                // Nếu không tìm thấy theo providerId, tìm theo email
                if (taiKhoan == null)
                {
                    taiKhoan = await _context.TaiKhoans
                        .FirstOrDefaultAsync(t => t.Email == email);
                }

                if (taiKhoan == null)
                {
                    // Tạo tài khoản mới
                    taiKhoan = new TaiKhoan
                    {
                        Email = email,
                        HoTen = name ?? email.Split('@')[0],
                        VaiTro = "User",
                        Provider = provider,
                        ProviderId = providerId,
                        MatKhau = "", // OAuth không cần mật khẩu
                        TrangThai = true,
                        NgayTao = DateTime.Now
                    };

                    _context.TaiKhoans.Add(taiKhoan);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Đã tạo tài khoản mới từ {provider}: {email}");
                }
                else
                {
                    // Cập nhật thông tin nếu cần
                    bool needUpdate = false;
                    
                    if (string.IsNullOrEmpty(taiKhoan.Provider))
                    {
                        taiKhoan.Provider = provider;
                        needUpdate = true;
                    }
                    
                    if (string.IsNullOrEmpty(taiKhoan.ProviderId) && !string.IsNullOrEmpty(providerId))
                    {
                        taiKhoan.ProviderId = providerId;
                        needUpdate = true;
                    }
                    
                    if (string.IsNullOrEmpty(taiKhoan.HoTen) && !string.IsNullOrEmpty(name))
                    {
                        taiKhoan.HoTen = name;
                        needUpdate = true;
                    }

                    if (needUpdate)
                    {
                        await _context.SaveChangesAsync();
                    }

                    if (!taiKhoan.TrangThai)
                    {
                        TempData["Error"] = "Tài khoản của bạn đã bị khóa!";
                        return RedirectToAction("Index");
                    }
                }

                // Tạo claims và đăng nhập
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

                _logger.LogInformation($"User {email} đã đăng nhập thành công qua {provider}.");

                // Chuyển hướng theo vai trò
                if (taiKhoan.VaiTro == "Admin" || taiKhoan.VaiTro == "NhanVien")
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi xử lý đăng nhập {provider}");
                TempData["Error"] = $"Đã xảy ra lỗi khi đăng nhập qua {provider}. Vui lòng thử lại!";
                return RedirectToAction("Index");
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

