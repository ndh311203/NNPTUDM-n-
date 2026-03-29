using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace spatc.Models
{
    public class CongViec
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên công việc không được để trống")]
        [Display(Name = "Tên công việc")]
        [StringLength(200)]
        public string TenCongViec { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        [Column(TypeName = "ntext")]
        public string? MoTa { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Quan hệ: Một công việc có nhiều lịch phân công
        public virtual ICollection<LichPhanCong>? LichPhanCongs { get; set; }
    }
}

