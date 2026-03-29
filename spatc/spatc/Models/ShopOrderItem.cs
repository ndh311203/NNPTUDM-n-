using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace spatc.Models
{
    public class ShopOrderItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Mã đơn hàng")]
        public int OrderId { get; set; }

        [Required]
        [Display(Name = "Mã sản phẩm")]
        public int ProductId { get; set; }

        [Required]
        [Display(Name = "Loại sản phẩm")]
        [StringLength(50)]
        public string ProductType { get; set; } = string.Empty; // pet, food, accessory

        [Required]
        [Display(Name = "Tên sản phẩm")]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Số lượng")]
        public int Quantity { get; set; } = 1;

        [Required]
        [Display(Name = "Giá")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ngày cập nhật")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Quan hệ: Một item thuộc về một đơn hàng
        [ForeignKey("OrderId")]
        public virtual ShopOrder? ShopOrder { get; set; }
    }
}

