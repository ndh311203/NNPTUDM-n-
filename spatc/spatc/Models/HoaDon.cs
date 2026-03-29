using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace spatc.Models
{
    public class HoaDon
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Ngày thanh toán")]
        public DateTime NgayThanhToan { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Tổng tiền không được để trống")]
        [Display(Name = "Tổng tiền")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TongTien { get; set; }

        [Display(Name = "Phương thức thanh toán")]
        [StringLength(50)]
        public string? PhuongThucThanhToan { get; set; } // Tiền mặt, Chuyển khoản, Thẻ...

        [Display(Name = "Ghi chú")]
        [StringLength(500)]
        public string? GhiChu { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Foreign Key: Liên kết với đơn đặt lịch (nullable - có thể tạo hóa đơn không cần đặt lịch)
        [Display(Name = "Đặt lịch")]
        public int? DatLichId { get; set; }

        // Navigation property
        [ForeignKey("DatLichId")]
        public virtual DatLich? DatLich { get; set; }

        // ========== CÁC TRƯỜNG CHO KHÁCH HÀNG ĐẾN QUÁN TRỰC TIẾP (WALK-IN) ==========
        [Display(Name = "Loại hóa đơn")]
        [StringLength(50)]
        public string LoaiHoaDon { get; set; } = "TuDatLich"; // "TuDatLich" hoặc "KhachDenQuan"

        [Display(Name = "Tên khách hàng")]
        [StringLength(100)]
        public string? KhachHangTen { get; set; } // Cho khách hàng walk-in

        [Display(Name = "Số điện thoại")]
        [StringLength(20)]
        public string? KhachHangSoDienThoai { get; set; } // Cho khách hàng walk-in

        [Display(Name = "Email")]
        [StringLength(100)]
        public string? KhachHangEmail { get; set; } // Cho khách hàng walk-in

        [Display(Name = "Tên dịch vụ")]
        [StringLength(200)]
        public string? TenDichVu { get; set; } // Tên dịch vụ cho walk-in

        [Display(Name = "Loại dịch vụ")]
        [StringLength(50)]
        public string? LoaiDichVu { get; set; } // "Grooming", "KhachSan", "Khac"
    }
}

