using System.ComponentModel.DataAnnotations;

namespace spatc.Models
{
    public class NhanVien
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [Display(Name = "Họ tên")]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vị trí không được để trống")]
        [Display(Name = "Vị trí")]
        [StringLength(100)]
        public string ViTri { get; set; } = string.Empty; // groomer, tắm, cắt tỉa...

        [Display(Name = "Số điện thoại")]
        [StringLength(20)]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? SoDienThoai { get; set; }

        [Display(Name = "Email")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        [Display(Name = "Ảnh nhân viên")]
        [StringLength(500)]
        public string? AnhNhanVien { get; set; }

        [Display(Name = "Lịch làm việc")]
        [StringLength(500)]
        public string? LichLamViec { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true; // true = đang làm việc, false = nghỉ

        // Quan hệ: Một nhân viên có nhiều đặt lịch
        public virtual ICollection<DatLich>? DatLiches { get; set; }
        
        // Quan hệ: Một nhân viên có nhiều lịch phân công
        public virtual ICollection<LichPhanCong>? LichPhanCongs { get; set; }
    }
}

