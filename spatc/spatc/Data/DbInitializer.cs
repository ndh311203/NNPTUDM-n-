using Microsoft.EntityFrameworkCore;
using spatc.Models;
using System.Security.Cryptography;
using System.Text;

namespace spatc.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Kiểm tra xem đã có admin chưa
            if (context.TaiKhoans.Any(t => t.Email == "nguyenduchuy311203@gmail.com"))
            {
                return; // Đã có admin rồi
            }

            // Tạo tài khoản admin
            var admin = new TaiKhoan
            {
                Email = "nguyenduchuy311203@gmail.com",
                MatKhau = HashPassword("satangking123"),
                HoTen = "Admin",
                VaiTro = "Admin",
                TrangThai = true,
                NgayTao = DateTime.Now
            };

            context.TaiKhoans.Add(admin);
            context.SaveChanges();
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}

