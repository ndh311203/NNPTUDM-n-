using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace spatc.Models
{
    /// <summary>
    /// Model thông báo trong hệ thống
    /// </summary>
    public class ThongBao
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// ID người nhận thông báo (có thể là Admin hoặc Khách hàng)
        /// Nếu null thì gửi cho tất cả admin
        /// </summary>
        [Display(Name = "Người nhận")]
        public int? NguoiNhanId { get; set; }

        /// <summary>
        /// Loại thông báo:
        /// ADMIN_BOOKING_NEW, ADMIN_BOOKING_CANCEL, ADMIN_BOOKING_UPDATE
        /// PET_CHECKIN, PET_IN_SERVICE, PET_DONE, PET_HOTEL_DAILY, PET_ISSUE
        /// </summary>
        [Required]
        [Display(Name = "Loại thông báo")]
        [StringLength(50)]
        public string LoaiThongBao { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Tiêu đề")]
        [StringLength(200)]
        public string TieuDe { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Nội dung")]
        [Column(TypeName = "ntext")]
        public string NoiDung { get; set; } = string.Empty;

        /// <summary>
        /// ID đặt lịch liên quan (nếu có)
        /// </summary>
        [Display(Name = "Đặt lịch")]
        public int? DatLichId { get; set; }

        /// <summary>
        /// ID thú cưng liên quan (nếu có)
        /// </summary>
        [Display(Name = "Thú cưng")]
        public int? ThuCungId { get; set; }

        /// <summary>
        /// Đã đọc chưa
        /// </summary>
        [Display(Name = "Đã đọc")]
        public bool DaDoc { get; set; } = false;

        /// <summary>
        /// Thời gian tạo
        /// </summary>
        [Display(Name = "Thời gian tạo")]
        public DateTime ThoiGianTao { get; set; } = DateTime.Now;

        /// <summary>
        /// Hình ảnh đính kèm (cho thông báo tình trạng sức khỏe)
        /// </summary>
        [Display(Name = "Hình ảnh")]
        [StringLength(500)]
        public string? HinhAnh { get; set; }

        // Navigation properties
        [ForeignKey("NguoiNhanId")]
        public virtual TaiKhoan? NguoiNhan { get; set; }

        [ForeignKey("DatLichId")]
        public virtual DatLich? DatLich { get; set; }

        [ForeignKey("ThuCungId")]
        public virtual ThuCung? ThuCung { get; set; }
    }

    /// <summary>
    /// Enum các loại thông báo
    /// </summary>
    public static class LoaiThongBao
    {
        // Thông báo gửi đến Admin
        public const string ADMIN_BOOKING_NEW = "ADMIN_BOOKING_NEW";
        public const string ADMIN_BOOKING_CANCEL = "ADMIN_BOOKING_CANCEL";
        public const string ADMIN_BOOKING_UPDATE = "ADMIN_BOOKING_UPDATE";
        public const string ADMIN_ORDER_NEW = "ADMIN_ORDER_NEW";

        // Thông báo gửi đến Khách hàng
        public const string PET_CHECKIN = "PET_CHECKIN";
        public const string PET_IN_SERVICE = "PET_IN_SERVICE";
        public const string PET_DONE = "PET_DONE";
        public const string PET_HOTEL_DAILY = "PET_HOTEL_DAILY";
        public const string PET_ISSUE = "PET_ISSUE";
        public const string PET_HEALTH_UPDATE = "PET_HEALTH_UPDATE"; // Tình trạng sức khỏe
    }
}

