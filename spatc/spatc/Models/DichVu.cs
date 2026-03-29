using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace spatc.Models
{
    public class DichVu
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên dịch vụ không được để trống")]
        [Display(Name = "Tên dịch vụ")]
        [StringLength(200)]
        public string TenDichVu { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        [Column(TypeName = "ntext")]
        public string? MoTa { get; set; }

        [Required(ErrorMessage = "Giá không được để trống")]
        [Display(Name = "Giá")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Gia { get; set; }

        [Display(Name = "Hình ảnh")]
        [StringLength(500)]
        public string? HinhAnh { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true; // true = hoạt động, false = ngừng

        [Display(Name = "Loại dịch vụ")]
        [StringLength(50)]
        public string LoaiDichVu { get; set; } = "Grooming"; // "Grooming" hoặc "KhachSan"

        // ========== CÁC TRƯỜNG CHO DỊCH VỤ GROOMING ==========
        [Display(Name = "Thời gian thực hiện (phút)")]
        [Column(TypeName = "int")]
        public int? ThoiGianThucHien { get; set; } // Thời gian ước tính (phút)

        [Display(Name = "Kích thước thú cưng")]
        [StringLength(50)]
        public string? KichThuocThuCung { get; set; } // "Nhỏ", "Trung", "Lớn"

        [Display(Name = "Loại dịch vụ Grooming")]
        [StringLength(100)]
        public string? LoaiDichVuGrooming { get; set; } // "Cơ bản", "Combo", "Nâng cao", "Spa" (giữ lại để tương thích)

        // ========== CÁC TRƯỜNG CHO DỊCH VỤ KHÁCH SẠN ==========
        [Display(Name = "Sức chứa / Giới hạn kích thước")]
        [StringLength(200)]
        public string? SucChua { get; set; } // VD: "dành cho thú < 25kg", "Tối đa 2 thú cưng"

        [Display(Name = "Loại phòng")]
        [StringLength(50)]
        public string? LoaiPhong { get; set; } // "Basic", "VIP", "SieuVIP"

        [Display(Name = "Bao gồm bữa ăn")]
        public bool? BaoGomBuaAn { get; set; }

        [Display(Name = "Dịch vụ bổ sung")]
        [Column(TypeName = "ntext")]
        public string? DichVuBoSung { get; set; } // VD: "tắm, dắt đi dạo, chăm sóc y tế"

        [Display(Name = "Thời gian nhận – trả thú cưng")]
        [StringLength(200)]
        public string? ThoiGianNhanTra { get; set; } // VD: "Nhận: 8h-18h, Trả: 8h-20h"

        // Quan hệ: Một dịch vụ có nhiều đặt lịch
        public virtual ICollection<DatLich>? DatLiches { get; set; }

        // Quan hệ: Một dịch vụ có nhiều đánh giá
        public virtual ICollection<ServiceReview>? ServiceReviews { get; set; }

        // Đánh giá trung bình và số lượt đánh giá
        [Display(Name = "Đánh giá trung bình")]
        [Column(TypeName = "decimal(3,2)")]
        public decimal? AverageRating { get; set; }

        [Display(Name = "Số lượt đánh giá")]
        public int ReviewCount { get; set; } = 0;
    }
}

