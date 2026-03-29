namespace spatc.Models
{
    public class PhanCongViewModel
    {
        public int Id { get; set; }
        public int NhanVienId { get; set; }
        public int CongViecId { get; set; }
        public DateTime NgayLam { get; set; }
        public string CaLam { get; set; } = string.Empty;
        public string TrangThai { get; set; } = string.Empty;
        public string? GhiChu { get; set; }

        // For display
        public string TenNhanVien { get; set; } = string.Empty;
        public string TenCongViec { get; set; } = string.Empty;
    }
}

