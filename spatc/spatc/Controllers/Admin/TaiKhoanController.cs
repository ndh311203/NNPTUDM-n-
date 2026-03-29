using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using spatc.Models;

namespace spatc.Controllers.Admin
{
    [Authorize]
    [Route("Admin/[controller]")]
    public class TaiKhoanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaiKhoanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/TaiKhoan
        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            // Kiểm tra quyền - chỉ Admin mới quản lý tài khoản
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (userRole != "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }

            return View("~/Views/Admin/TaiKhoan/Index.cshtml", await _context.TaiKhoans.OrderByDescending(t => t.NgayTao).ToListAsync());
        }

        // GET: Admin/TaiKhoan/Details/5
        [HttpGet]
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            // Kiểm tra quyền
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (userRole != "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }

            if (id == null)
            {
                return NotFound();
            }

            var taiKhoan = await _context.TaiKhoans.FirstOrDefaultAsync(m => m.Id == id);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/TaiKhoan/Details.cshtml", taiKhoan);
        }

        // GET: Admin/TaiKhoan/Create
        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            // Kiểm tra quyền
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (userRole != "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }

            return View("~/Views/Admin/TaiKhoan/Create.cshtml");
        }

        // POST: Admin/TaiKhoan/Create
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Email,MatKhau,HoTen,VaiTro,TrangThai")] TaiKhoan taiKhoan, string matKhau)
        {
            // Kiểm tra email đã tồn tại chưa
            if (await _context.TaiKhoans.AnyAsync(t => t.Email == taiKhoan.Email))
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng!");
            }

            if (ModelState.IsValid)
            {
                // Hash mật khẩu trước khi lưu
                taiKhoan.MatKhau = HashPassword(matKhau);
                taiKhoan.NgayTao = DateTime.Now;
                _context.Add(taiKhoan);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm tài khoản thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Admin/TaiKhoan/Create.cshtml", taiKhoan);
        }

        // GET: Admin/TaiKhoan/Edit/5
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            // Kiểm tra quyền
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (userRole != "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }

            if (id == null)
            {
                return NotFound();
            }

            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan == null)
            {
                return NotFound();
            }
            return View("~/Views/Admin/TaiKhoan/Edit.cshtml", taiKhoan);
        }

        // POST: Admin/TaiKhoan/Edit/5
        [HttpPost]
        [Route("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,HoTen,VaiTro,TrangThai,NgayTao")] TaiKhoan taiKhoan, string? matKhau)
        {
            if (id != taiKhoan.Id)
            {
                return NotFound();
            }

            // Loại bỏ validation cho MatKhau vì khi edit có thể không đổi mật khẩu
            ModelState.Remove("MatKhau");

            // Kiểm tra email đã tồn tại chưa (trừ chính nó)
            var existingAccount = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.Email == taiKhoan.Email && t.Id != id);
            if (existingAccount != null)
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng!");
            }

            // Validate các trường bắt buộc
            if (string.IsNullOrWhiteSpace(taiKhoan.Email))
            {
                ModelState.AddModelError("Email", "Email không được để trống");
            }

            if (string.IsNullOrWhiteSpace(taiKhoan.VaiTro))
            {
                ModelState.AddModelError("VaiTro", "Vai trò không được để trống");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Load entity từ database để track changes đúng cách
                    var currentAccount = await _context.TaiKhoans.FindAsync(id);
                    if (currentAccount == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật từng property
                    currentAccount.Email = taiKhoan.Email?.Trim() ?? currentAccount.Email;
                    currentAccount.HoTen = taiKhoan.HoTen?.Trim();
                    currentAccount.VaiTro = taiKhoan.VaiTro?.Trim() ?? "User"; // Đảm bảo VaiTro được cập nhật
                    currentAccount.TrangThai = taiKhoan.TrangThai;
                    currentAccount.NgayTao = taiKhoan.NgayTao;

                    // Nếu có nhập mật khẩu mới, hash và cập nhật
                    if (!string.IsNullOrWhiteSpace(matKhau))
                    {
                        currentAccount.MatKhau = HashPassword(matKhau);
                    }
                    // Nếu không nhập mật khẩu, giữ nguyên (không cần làm gì)

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật tài khoản thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaiKhoanExists(taiKhoan.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi khi cập nhật: {ex.Message}");
                }
            }
            
            // Nếu có lỗi, trả về view với model
            return View("~/Views/Admin/TaiKhoan/Edit.cshtml", taiKhoan);
        }

        // GET: Admin/TaiKhoan/Delete/5
        [HttpGet]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            // Kiểm tra quyền
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (userRole != "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }

            if (id == null)
            {
                return NotFound();
            }

            var taiKhoan = await _context.TaiKhoans.FirstOrDefaultAsync(m => m.Id == id);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/TaiKhoan/Delete.cshtml", taiKhoan);
        }

        // POST: Admin/TaiKhoan/Delete/5
        [HttpPost]
        [Route("Delete/{id}")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Kiểm tra quyền
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (userRole != "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }

            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan != null)
            {
                _context.TaiKhoans.Remove(taiKhoan);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa tài khoản thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TaiKhoanExists(int id)
        {
            return _context.TaiKhoans.Any(e => e.Id == id);
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

