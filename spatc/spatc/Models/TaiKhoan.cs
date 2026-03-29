using System.ComponentModel.DataAnnotations;

namespace spatc.Models
{
    public class TaiKhoan
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [Display(Name = "Email")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [Display(Name = "Mật khẩu")]
        [StringLength(255)]
        public string MatKhau { get; set; } = string.Empty; // Lưu dạng hash

        [Display(Name = "Họ tên")]
        [StringLength(100)]
        public string? HoTen { get; set; }

        [Display(Name = "Vai trò")]
        [StringLength(50)]
        public string VaiTro { get; set; } = "User"; // Admin, NhanVien (Staff), User

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true; // true = hoạt động, false = khóa

        [Display(Name = "Provider")]
        [StringLength(50)]
        public string? Provider { get; set; } // "Facebook", "Google", null = local

        [Display(Name = "Provider ID")]
        [StringLength(255)]
        public string? ProviderId { get; set; } // ID từ provider

        // Navigation property: Một tài khoản có thể có một khách hàng
        public virtual KhachHang? KhachHang { get; set; }
    }
}

