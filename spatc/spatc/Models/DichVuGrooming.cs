using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace spatc.Models
{
    public class DichVuGrooming
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên dịch vụ không được để trống")]
        [Display(Name = "Tên dịch vụ")]
        [StringLength(200)]
        public string TenDichVu { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        [Column(TypeName = "ntext")]
        public string? MoTa { get; set; }

        [Required(ErrorMessage = "Giá không được để trống")]
        [Display(Name = "Giá")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Gia { get; set; }

        [Display(Name = "Thời gian thực hiện (phút)")]
        [Column(TypeName = "int")]
        public int? ThoiGianThucHien { get; set; } // Thời gian ước tính (phút)

        [Display(Name = "Hình ảnh")]
        [StringLength(500)]
        public string? HinhAnh { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true; // true = đang sử dụng, false = tạm ngưng

        // Quan hệ: Một dịch vụ grooming có nhiều đặt lịch
        public virtual ICollection<DatLich>? DatLiches { get; set; }
    }
}

