using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace spatc.Models
{
    public class CaLam
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên ca không được để trống")]
        [Display(Name = "Tên ca")]
        [StringLength(100)]
        public string TenCa { get; set; } = string.Empty;

        [Display(Name = "Giờ bắt đầu")]
        [DataType(DataType.Time)]
        public TimeSpan GioBatDau { get; set; }

        [Display(Name = "Giờ kết thúc")]
        [DataType(DataType.Time)]
        public TimeSpan GioKetThuc { get; set; }

        [Display(Name = "Mô tả")]
        [StringLength(500)]
        public string? MoTa { get; set; }
    }
}

