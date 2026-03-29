using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace spatc.Models
{
    public class DatLich
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Thời gian hẹn không được để trống")]
        [Display(Name = "Thời gian hẹn")]
        public DateTime ThoiGianHen { get; set; }

        [Display(Name = "Ghi chú")]
        [Column(TypeName = "ntext")]
        public string? GhiChu { get; set; }

        [Required]
        [Display(Name = "Trạng thái")]
        [StringLength(50)]
        public string TrangThai { get; set; } = "Chờ"; // Chờ, Đang làm, Hoàn thành, Hủy

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Foreign Key: Thuộc thú cưng nào
        [Required]
        [Display(Name = "Thú cưng")]
        public int ThuCungId { get; set; }

        // Foreign Key: Dịch vụ nào (có thể null nếu dùng Grooming hoặc KhachSan)
        [Display(Name = "Dịch vụ")]
        public int? DichVuId { get; set; }

        // Foreign Key: Dịch vụ Grooming (có thể null)
        [Display(Name = "Dịch vụ Grooming")]
        public int? DichVuGroomingId { get; set; }

        // Foreign Key: Phòng khách sạn (có thể null)
        [Display(Name = "Phòng khách sạn")]
        public int? PhongKhachSanId { get; set; }

        // Foreign Key: Nhân viên thực hiện (có thể null nếu chưa gán)
        [Display(Name = "Nhân viên")]
        public int? NhanVienId { get; set; }

        // Thời gian kết thúc (cho khách sạn)
        [Display(Name = "Thời gian kết thúc")]
        public DateTime? ThoiGianKetThuc { get; set; }

        // Ghi chú đặc biệt (cho khách sạn: thuốc, ăn uống, tính cách...)
        [Display(Name = "Ghi chú đặc biệt")]
        [Column(TypeName = "ntext")]
        public string? GhiChuDacBiet { get; set; }

        // Navigation properties
        [ForeignKey("ThuCungId")]
        public virtual ThuCung? ThuCung { get; set; }

        [ForeignKey("DichVuId")]
        public virtual DichVu? DichVu { get; set; }

        [ForeignKey("DichVuGroomingId")]
        public virtual DichVuGrooming? DichVuGrooming { get; set; }

        [ForeignKey("PhongKhachSanId")]
        public virtual PhongKhachSan? PhongKhachSan { get; set; }

        [ForeignKey("NhanVienId")]
        public virtual NhanVien? NhanVien { get; set; }

        // Quan hệ: Một đặt lịch có một hóa đơn
        public virtual HoaDon? HoaDon { get; set; }

        // Quan hệ: Một đặt lịch có nhiều dịch vụ Grooming (many-to-many)
        public virtual ICollection<DatLichDichVuGrooming> DatLichDichVuGroomings { get; set; } = new List<DatLichDichVuGrooming>();
    }
}

