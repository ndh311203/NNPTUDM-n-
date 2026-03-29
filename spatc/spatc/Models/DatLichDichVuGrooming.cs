using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace spatc.Models
{
    // Bảng trung gian để lưu nhiều dịch vụ Grooming cho một đặt lịch
    public class DatLichDichVuGrooming
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Đặt lịch")]
        public int DatLichId { get; set; }

        [Required]
        [Display(Name = "Dịch vụ Grooming")]
        public int DichVuGroomingId { get; set; }

        // Navigation properties
        [ForeignKey("DatLichId")]
        public virtual DatLich? DatLich { get; set; }

        [ForeignKey("DichVuGroomingId")]
        public virtual DichVuGrooming? DichVuGrooming { get; set; }
    }
}

