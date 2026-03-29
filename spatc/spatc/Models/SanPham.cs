using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace spatc.Models
{
    public class SanPham
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [Display(Name = "Tên sản phẩm")]
        [StringLength(200)]
        public string TenSanPham { get; set; } = string.Empty;

        [Required(ErrorMessage = "Loại sản phẩm không được để trống")]
        [Display(Name = "Loại sản phẩm")]
        [StringLength(50)]
        public string LoaiSanPham { get; set; } = "ThucAn"; // "ThucAn" hoặc "DoChoi"

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

        [Display(Name = "Số lượng tồn kho")]
        [Column(TypeName = "int")]
        public int SoLuongTon { get; set; } = 0;

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true; // true = còn hàng, false = hết hàng

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
}

