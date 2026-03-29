    using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace spatc.Models
{
    public class FoodProduct
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [Display(Name = "Tên sản phẩm")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Thương hiệu")]
        [StringLength(100)]
        public string? Brand { get; set; }

        [Required(ErrorMessage = "Loại thức ăn không được để trống")]
        [Display(Name = "Loại thức ăn")]
        [StringLength(100)]
        public string Type { get; set; } = string.Empty; 

        [Display(Name = "Trọng lượng")]
        [StringLength(50)]
        public string? Weight { get; set; }

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

        [Display(Name = "Hình ảnh chính")]
        [StringLength(500)]
        public string? Image { get; set; }

        [Display(Name = "Gallery ảnh")]
        [Column(TypeName = "ntext")]
        public string? GalleryImages { get; set; } // JSON array hoặc comma-separated URLs

        [Display(Name = "Số lượng tồn kho")]
        public int SoLuongTon { get; set; } = 0;

        [Display(Name = "SKU")]
        [StringLength(100)]
        public string? SKU { get; set; }

        [Display(Name = "Danh mục")]
        public int? CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public ProductCategory? Category { get; set; }

        [Display(Name = "Giảm giá (%)")]
        [Column(TypeName = "decimal(5,2)")]
        public decimal DiscountPercent { get; set; } = 0;

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true; // true = còn hàng, false = hết hàng

        [Display(Name = "Đã xóa")]
        public bool IsDeleted { get; set; } = false; // Soft delete

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ngày cập nhật")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}

