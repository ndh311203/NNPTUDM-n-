using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace spatc.Models
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Người gửi")]
        public int NguoiGuiId { get; set; }

        [Display(Name = "Người nhận")]
        public int? NguoiNhanId { get; set; }

        [Required]
        [Display(Name = "Nội dung")]
        [Column(TypeName = "ntext")]
        public string NoiDung { get; set; } = string.Empty;

        [Display(Name = "Loại tin nhắn")]
        [StringLength(50)]
        public string LoaiTinNhan { get; set; } = "text";

        [Display(Name = "Đã đọc")]
        public bool DaDoc { get; set; } = false;

        [Display(Name = "Thời gian gửi")]
        public DateTime ThoiGianGui { get; set; } = DateTime.Now;

        [Display(Name = "IsAdmin")]
        public bool IsAdmin { get; set; } = false;

        [ForeignKey("NguoiGuiId")]
        public virtual TaiKhoan? NguoiGui { get; set; }

        [ForeignKey("NguoiNhanId")]
        public virtual TaiKhoan? NguoiNhan { get; set; }
    }
}

