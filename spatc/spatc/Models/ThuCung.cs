using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace spatc.Models
{
    public class ThuCung
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên thú cưng không được để trống")]
        [Display(Name = "Tên thú cưng")]
        [StringLength(100)]
        public string TenThuCung { get; set; } = string.Empty;

        [Required(ErrorMessage = "Loại không được để trống")]
        [Display(Name = "Loại")]
        [StringLength(50)]
        public string Loai { get; set; } = string.Empty; // Chó / Mèo

        [Display(Name = "Giống")]
        [StringLength(100)]
        public string? Giong { get; set; }

        [Display(Name = "Tuổi")]
        public int? Tuoi { get; set; }

        [Display(Name = "Cân nặng (kg)")]
        [Column(TypeName = "decimal(5,2)")]
        public decimal? CanNang { get; set; }

        [Display(Name = "Ghi chú sức khỏe")]
        [Column(TypeName = "ntext")]
        public string? GhiChuSucKhoe { get; set; }

        [Display(Name = "Hình ảnh")]
        [StringLength(500)]
        public string? HinhAnh { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Foreign Key: Thuộc khách hàng nào
        [Required]
        [Display(Name = "Khách hàng")]
        public int KhachHangId { get; set; }

        // Navigation property
        [ForeignKey("KhachHangId")]
        public virtual KhachHang? KhachHang { get; set; }

        // Quan hệ: Một thú cưng có nhiều đặt lịch
        public virtual ICollection<DatLich>? DatLiches { get; set; }
    }
}

