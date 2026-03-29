using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace spatc.Models
{
    public class PhongKhachSan
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên phòng không được để trống")]
        [Display(Name = "Tên phòng")]
        [StringLength(200)]
        public string TenPhong { get; set; } = string.Empty;

        [Required(ErrorMessage = "Loại phòng không được để trống")]
        [Display(Name = "Loại phòng")]
        [StringLength(50)]
        public string LoaiPhong { get; set; } = "Thường"; // "Thường", "VIP", "SieuVIP", "Ghep"

        [Required(ErrorMessage = "Giá theo giờ không được để trống")]
        [Display(Name = "Giá theo giờ")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal GiaTheoGio { get; set; }

        [Required(ErrorMessage = "Giá theo ngày không được để trống")]
        [Display(Name = "Giá theo ngày")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal GiaTheoNgay { get; set; }

        [Required(ErrorMessage = "Số lượng tối đa không được để trống")]
        [Display(Name = "Số lượng thú cưng tối đa")]
        [Column(TypeName = "int")]
        public int SoLuongToiDa { get; set; } = 1;

        [Display(Name = "Tiện nghi")]
        [Column(TypeName = "ntext")]
        public string? TienNghi { get; set; } // Danh sách tiện nghi: nệm, camera, máy lạnh...

        [Display(Name = "Hình ảnh")]
        [StringLength(500)]
        public string? HinhAnh { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        [Display(Name = "Trạng thái")]
        [StringLength(50)]
        public string TrangThai { get; set; } = "Trống"; // "Trống", "Đang sử dụng", "Ngừng hoạt động"

        // Quan hệ: Một phòng có nhiều đặt lịch lưu trú
        public virtual ICollection<DatLich>? DatLiches { get; set; }
    }
}

