using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace spatc.Models
{
    public class ServiceReview
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Mã dịch vụ")]
        public int DichVuId { get; set; }

        [Required(ErrorMessage = "Đánh giá không được để trống")]
        [Display(Name = "Đánh giá")]
        [Range(1, 5, ErrorMessage = "Đánh giá phải từ 1 đến 5 sao")]
        public int Rating { get; set; } = 5;

        [Display(Name = "Bình luận")]
        [Column(TypeName = "ntext")]
        public string? Comment { get; set; }

        [Display(Name = "Hình ảnh")]
        [Column(TypeName = "ntext")]
        public string? Images { get; set; } // Comma-separated URLs (tối đa 3 ảnh)

        [Required]
        [Display(Name = "Tên khách hàng")]
        [StringLength(200)]
        public string CustomerName { get; set; } = string.Empty;

        [Display(Name = "Email khách hàng")]
        [StringLength(200)]
        public string? CustomerEmail { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ngày cập nhật")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Trạng thái")]
        [StringLength(20)]
        public string Status { get; set; } = "approved"; // approved, pending, rejected

        // Navigation property
        [ForeignKey("DichVuId")]
        public virtual DichVu? DichVu { get; set; }
    }
}











