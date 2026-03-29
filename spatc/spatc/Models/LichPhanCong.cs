using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace spatc.Models
{
    public class LichPhanCong
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nhân viên không được để trống")]
        [Display(Name = "Nhân viên")]
        public int NhanVienId { get; set; }

        [Required(ErrorMessage = "Công việc không được để trống")]
        [Display(Name = "Công việc")]
        public int CongViecId { get; set; }

        [Required(ErrorMessage = "Ngày làm không được để trống")]
        [Display(Name = "Ngày làm")]
        [DataType(DataType.Date)]
        public DateTime NgayLam { get; set; }

        [Display(Name = "Ca làm")]
        [StringLength(50)]
        public string CaLam { get; set; } = "Ca sáng"; // Ca sáng, Ca chiều, Full-time

        [Display(Name = "Trạng thái")]
        [StringLength(50)]
        public string TrangThai { get; set; } = "Chưa làm"; // Chưa làm, Đang làm, Hoàn thành, Nghỉ

        [Display(Name = "Ghi chú")]
        [Column(TypeName = "ntext")]
        public string? GhiChu { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("NhanVienId")]
        public virtual NhanVien? NhanVien { get; set; }

        [ForeignKey("CongViecId")]
        public virtual CongViec? CongViec { get; set; }
    }
}

