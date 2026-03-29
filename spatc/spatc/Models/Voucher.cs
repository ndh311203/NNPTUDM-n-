using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace spatc.Models
{
    public class Voucher
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Mã voucher không được để trống")]
        [Display(Name = "Mã voucher")]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Loại giảm giá")]
        [StringLength(20)]
        public string Type { get; set; } = "percent"; // percent, cash

        [Required]
        [Display(Name = "Giá trị")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Value { get; set; }

        [Display(Name = "Đơn hàng tối thiểu")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MinOrderAmount { get; set; } = 0;

        [Display(Name = "Giảm tối đa")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MaxDiscount { get; set; }

        [Display(Name = "Ngày bắt đầu")]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Display(Name = "Ngày kết thúc")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Trạng thái")]
        [StringLength(20)]
        public string Status { get; set; } = "active"; // active, inactive

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ngày cập nhật")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}

