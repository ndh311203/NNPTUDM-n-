using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace spatc.Models
{
    public class PetProduct
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên thú cưng không được để trống")]
        [Display(Name = "Tên thú cưng")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giống loài không được để trống")]
        [Display(Name = "Giống loài")]
        [StringLength(100)]
        public string Species { get; set; } = string.Empty; // Dog, Cat, Hamster, ...

        [Display(Name = "Tuổi")]
        [StringLength(50)]
        public string? Age { get; set; }

        [Display(Name = "Giới tính")]
        [StringLength(20)]
        public string? Gender { get; set; } // Đực, Cái

        [Display(Name = "Tình trạng sức khỏe")]
        [StringLength(200)]
        public string? HealthStatus { get; set; }

        [Required(ErrorMessage = "Giá bán không được để trống")]
        [Display(Name = "Giá bán")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Display(Name = "Giá cũ")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? OldPrice { get; set; }

        [Display(Name = "Đánh giá trung bình")]
        [Column(TypeName = "decimal(3,2)")]
        public decimal AverageRating { get; set; } = 0;

        [Display(Name = "Số lượt đánh giá")]
        public int ReviewCount { get; set; } = 0;

        [Display(Name = "Số lượt đã bán")]
        public int SoldCount { get; set; } = 0;

        [Display(Name = "Mô tả")]
        [Column(TypeName = "ntext")]
        public string? Description { get; set; }

        [Display(Name = "Hình ảnh")]
        [Column(TypeName = "ntext")]
        public string? Images { get; set; } // JSON array hoặc comma-separated URLs (tối thiểu 4 ảnh)

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true; // true = còn bán, false = đã bán

        [Display(Name = "Đã xóa")]
        public bool IsDeleted { get; set; } = false; // Soft delete

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ngày cập nhật")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}

