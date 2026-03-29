using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace spatc.Models
{
    public class KhachHang
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [Display(Name = "Họ tên")]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Display(Name = "Số điện thoại")]
        [StringLength(20)]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Display(Name = "Email")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        [Display(Name = "Địa chỉ")]
        [StringLength(500)]
        public string? DiaChi { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        [Display(Name = "Ghi chú")]
        [StringLength(1000)]
        public string? GhiChu { get; set; }

        // Foreign Key: Liên kết với tài khoản
        [Display(Name = "Tài khoản")]
        public int? TaiKhoanId { get; set; }

        // Navigation property
        [ForeignKey("TaiKhoanId")]
        public virtual TaiKhoan? TaiKhoan { get; set; }

        // Quan hệ: Một khách hàng có nhiều thú cưng
        public virtual ICollection<ThuCung>? ThuCungs { get; set; }
    }
}

