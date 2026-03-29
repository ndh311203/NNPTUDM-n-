using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace spatc.Models
{
    public class AccessoryProduct
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [Display(Name = "Tên sản phẩm")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phân loại không được để trống")]
        [Display(Name = "Phân loại")]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty; // Lồng, Vòng cổ, Dây dắt, Đồ chơi, Đồ tắm, ...

        [Display(Name = "Chất liệu")]
        [StringLength(100)]
        public string? Material { get; set; }

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

        [Display(Name = "Danh mục sản phẩm")]
        public int? ProductCategoryId { get; set; }

        [ForeignKey("ProductCategoryId")]
        public ProductCategory? ProductCategory { get; set; }

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

