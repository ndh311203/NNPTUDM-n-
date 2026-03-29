using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace spatc.Models
{
    public class ShopOrder
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [Display(Name = "Họ tên")]
        [StringLength(200)]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Display(Name = "Số điện thoại")]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        [StringLength(200)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        [Display(Name = "Địa chỉ")]
        [StringLength(500)]
        public string Address { get; set; } = string.Empty;

        [Display(Name = "Ghi chú")]
        [Column(TypeName = "ntext")]
        public string? Note { get; set; }

        [Required]
        [Display(Name = "Tổng tiền")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [Display(Name = "Phương thức thanh toán")]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = "COD"; // COD, BankTransfer, VietQR

        [Required]
        [Display(Name = "Trạng thái")]
        [StringLength(50)]
        public string Status { get; set; } = "Chờ xác nhận"; // Chờ xác nhận, Đang giao, Hoàn thành, Hủy

        [Display(Name = "Trạng thái thanh toán")]
        [StringLength(50)]
        public string? PaymentStatus { get; set; } // waiting_payment, Paid, Canceled, Failed

        [Display(Name = "Mã voucher")]
        [StringLength(50)]
        public string? VoucherCode { get; set; }

        [Display(Name = "Số tiền giảm giá")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; } = 0;

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ngày cập nhật")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Quan hệ: Một đơn hàng có nhiều sản phẩm
        public virtual ICollection<ShopOrderItem>? ShopOrderItems { get; set; }
    }
}

