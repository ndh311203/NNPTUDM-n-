using Microsoft.EntityFrameworkCore;

namespace spatc.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet cho các bảng
        public DbSet<DichVu> DichVus { get; set; }
        public DbSet<DichVuGrooming> DichVuGroomings { get; set; }
        public DbSet<PhongKhachSan> PhongKhachSans { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<ThuCung> ThuCungs { get; set; }
        public DbSet<DatLich> DatLiches { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<DatLichDichVuGrooming> DatLichDichVuGroomings { get; set; }
        
        // Phân công công việc
        public DbSet<CongViec> CongViecs { get; set; }
        public DbSet<LichPhanCong> LichPhanCongs { get; set; }
        public DbSet<CaLam> CaLams { get; set; }
        
        // Pet Shop - Cửa hàng thú cưng
        public DbSet<PetProduct> PetProducts { get; set; }
        public DbSet<FoodProduct> FoodProducts { get; set; }
        public DbSet<AccessoryProduct> AccessoryProducts { get; set; }
        
        // Shop Orders - Đơn hàng Pet Shop
        public DbSet<ShopOrder> ShopOrders { get; set; }
        public DbSet<ShopOrderItem> ShopOrderItems { get; set; }
        
        // Vouchers - Mã giảm giá
        public DbSet<Voucher> Vouchers { get; set; }
        
        // Product Reviews - Đánh giá sản phẩm
        public DbSet<ProductReview> ProductReviews { get; set; }
        
        // Service Reviews - Đánh giá dịch vụ
        public DbSet<ServiceReview> ServiceReviews { get; set; }
        
        // Product Categories - Danh mục sản phẩm
        public DbSet<ProductCategory> ProductCategories { get; set; }
        
        // Thông báo
        public DbSet<ThongBao> ThongBaos { get; set; }
        
        // Chat Messages
        public DbSet<ChatMessage> ChatMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình quan hệ và ràng buộc

            // TaiKhoan -> KhachHang (1-1, optional)
            modelBuilder.Entity<KhachHang>()
                .HasOne(k => k.TaiKhoan)
                .WithOne(t => t.KhachHang)
                .HasForeignKey<KhachHang>(k => k.TaiKhoanId)
                .OnDelete(DeleteBehavior.SetNull); // Nếu xóa tài khoản, set null

            // KhachHang -> ThuCung (1-nhiều)
            modelBuilder.Entity<ThuCung>()
                .HasOne(t => t.KhachHang)
                .WithMany(k => k.ThuCungs)
                .HasForeignKey(t => t.KhachHangId)
                .OnDelete(DeleteBehavior.Restrict); // Không cho xóa khách hàng nếu còn thú cưng

            // ThuCung -> DatLich (1-nhiều)
            modelBuilder.Entity<DatLich>()
                .HasOne(d => d.ThuCung)
                .WithMany(t => t.DatLiches)
                .HasForeignKey(d => d.ThuCungId)
                .OnDelete(DeleteBehavior.Restrict);

            // DichVu -> DatLich (1-nhiều, optional)
            modelBuilder.Entity<DatLich>()
                .HasOne(d => d.DichVu)
                .WithMany(dv => dv.DatLiches)
                .HasForeignKey(d => d.DichVuId)
                .OnDelete(DeleteBehavior.Restrict);

            // DichVuGrooming -> DatLich (1-nhiều, optional)
            modelBuilder.Entity<DatLich>()
                .HasOne(d => d.DichVuGrooming)
                .WithMany(dg => dg.DatLiches)
                .HasForeignKey(d => d.DichVuGroomingId)
                .OnDelete(DeleteBehavior.Restrict);

            // PhongKhachSan -> DatLich (1-nhiều, optional)
            modelBuilder.Entity<DatLich>()
                .HasOne(d => d.PhongKhachSan)
                .WithMany(p => p.DatLiches)
                .HasForeignKey(d => d.PhongKhachSanId)
                .OnDelete(DeleteBehavior.Restrict);

            // NhanVien -> DatLich (1-nhiều, optional)
            modelBuilder.Entity<DatLich>()
                .HasOne(d => d.NhanVien)
                .WithMany(n => n.DatLiches)
                .HasForeignKey(d => d.NhanVienId)
                .OnDelete(DeleteBehavior.SetNull); // Nếu xóa nhân viên, set null

            // NhanVien -> LichPhanCong (1-nhiều)
            modelBuilder.Entity<LichPhanCong>()
                .HasOne(l => l.NhanVien)
                .WithMany(n => n.LichPhanCongs)
                .HasForeignKey(l => l.NhanVienId)
                .OnDelete(DeleteBehavior.Restrict);

            // CongViec -> LichPhanCong (1-nhiều)
            modelBuilder.Entity<LichPhanCong>()
                .HasOne(l => l.CongViec)
                .WithMany(c => c.LichPhanCongs)
                .HasForeignKey(l => l.CongViecId)
                .OnDelete(DeleteBehavior.Restrict);

            // DatLich -> HoaDon (1-1, optional - có thể tạo hóa đơn không cần đặt lịch)
            modelBuilder.Entity<HoaDon>()
                .HasOne(h => h.DatLich)
                .WithOne(d => d.HoaDon)
                .HasForeignKey<HoaDon>(h => h.DatLichId)
                .OnDelete(DeleteBehavior.SetNull); // Set null nếu xóa đặt lịch (cho phép hóa đơn walk-in tồn tại độc lập)

            // DatLich -> DatLichDichVuGrooming (1-nhiều)
            modelBuilder.Entity<DatLichDichVuGrooming>()
                .HasOne(dlg => dlg.DatLich)
                .WithMany(d => d.DatLichDichVuGroomings)
                .HasForeignKey(dlg => dlg.DatLichId)
                .OnDelete(DeleteBehavior.Cascade);

            // DichVuGrooming -> DatLichDichVuGrooming (1-nhiều)
            modelBuilder.Entity<DatLichDichVuGrooming>()
                .HasOne(dlg => dlg.DichVuGrooming)
                .WithMany()
                .HasForeignKey(dlg => dlg.DichVuGroomingId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mặc định cho các trường
            modelBuilder.Entity<DichVu>()
                .Property(d => d.Gia)
                .HasPrecision(18, 2);

            modelBuilder.Entity<DichVuGrooming>()
                .Property(d => d.Gia)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PhongKhachSan>()
                .Property(p => p.GiaTheoGio)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PhongKhachSan>()
                .Property(p => p.GiaTheoNgay)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ThuCung>()
                .Property(t => t.CanNang)
                .HasPrecision(5, 2);

            modelBuilder.Entity<HoaDon>()
                .Property(h => h.TongTien)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SanPham>()
                .Property(s => s.Gia)
                .HasPrecision(18, 2);

            // Cấu hình precision cho Pet Shop
            modelBuilder.Entity<PetProduct>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<FoodProduct>()
                .Property(f => f.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<AccessoryProduct>()
                .Property(a => a.Price)
                .HasPrecision(18, 2);

            // Cấu hình precision cho Shop Orders
            modelBuilder.Entity<ShopOrder>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ShopOrderItem>()
                .Property(i => i.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ShopOrder>()
                .Property(o => o.DiscountAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Voucher>()
                .Property(v => v.Value)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Voucher>()
                .Property(v => v.MinOrderAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Voucher>()
                .Property(v => v.MaxDiscount)
                .HasPrecision(18, 2);

            // Cấu hình precision cho giá cũ và rating
            modelBuilder.Entity<PetProduct>()
                .Property(p => p.OldPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PetProduct>()
                .Property(p => p.AverageRating)
                .HasPrecision(3, 2);

            modelBuilder.Entity<FoodProduct>()
                .Property(f => f.OldPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<FoodProduct>()
                .Property(f => f.AverageRating)
                .HasPrecision(3, 2);

            modelBuilder.Entity<AccessoryProduct>()
                .Property(a => a.OldPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<AccessoryProduct>()
                .Property(a => a.AverageRating)
                .HasPrecision(3, 2);

            // Index cho ProductReview
            modelBuilder.Entity<ProductReview>()
                .HasIndex(r => new { r.ProductId, r.ProductType });

            // Index cho ServiceReview
            modelBuilder.Entity<ServiceReview>()
                .HasIndex(r => r.DichVuId);

            // Cấu hình precision cho AverageRating của DichVu
            modelBuilder.Entity<DichVu>()
                .Property(d => d.AverageRating)
                .HasPrecision(3, 2);

            // Index cho Voucher Code (unique)
            modelBuilder.Entity<Voucher>()
                .HasIndex(v => v.Code)
                .IsUnique();

            // ShopOrder -> ShopOrderItem (1-nhiều)
            modelBuilder.Entity<ShopOrderItem>()
                .HasOne(i => i.ShopOrder)
                .WithMany(o => o.ShopOrderItems)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index cho các trường thường xuyên tìm kiếm
            modelBuilder.Entity<KhachHang>()
                .HasIndex(k => k.SoDienThoai)
                .IsUnique();

            modelBuilder.Entity<DatLich>()
                .HasIndex(d => d.ThoiGianHen);

            // Index cho Email trong TaiKhoan (unique)
            modelBuilder.Entity<TaiKhoan>()
                .HasIndex(t => t.Email)
                .IsUnique();

            // Cấu hình precision cho DiscountPercent
            modelBuilder.Entity<FoodProduct>()
                .Property(f => f.DiscountPercent)
                .HasPrecision(5, 2);

            modelBuilder.Entity<AccessoryProduct>()
                .Property(a => a.DiscountPercent)
                .HasPrecision(5, 2);

            // Index cho ProductCategory Slug (unique)
            modelBuilder.Entity<ProductCategory>()
                .HasIndex(c => c.Slug)
                .IsUnique();

            // Index cho SKU (optional unique)
            modelBuilder.Entity<FoodProduct>()
                .HasIndex(f => f.SKU)
                .IsUnique()
                .HasFilter("[SKU] IS NOT NULL");

            modelBuilder.Entity<AccessoryProduct>()
                .HasIndex(a => a.SKU)
                .IsUnique()
                .HasFilter("[SKU] IS NOT NULL");

            // Soft delete filter - chỉ lấy records chưa bị xóa
            modelBuilder.Entity<PetProduct>()
                .HasQueryFilter(p => !p.IsDeleted);

            modelBuilder.Entity<FoodProduct>()
                .HasQueryFilter(f => !f.IsDeleted);

            modelBuilder.Entity<AccessoryProduct>()
                .HasQueryFilter(a => !a.IsDeleted);

            // Cấu hình quan hệ cho ThongBao
            modelBuilder.Entity<ThongBao>()
                .HasOne(t => t.NguoiNhan)
                .WithMany()
                .HasForeignKey(t => t.NguoiNhanId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ThongBao>()
                .HasOne(t => t.DatLich)
                .WithMany()
                .HasForeignKey(t => t.DatLichId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ThongBao>()
                .HasOne(t => t.ThuCung)
                .WithMany()
                .HasForeignKey(t => t.ThuCungId)
                .OnDelete(DeleteBehavior.SetNull);

            // Index cho ThongBao
            modelBuilder.Entity<ThongBao>()
                .HasIndex(t => new { t.NguoiNhanId, t.DaDoc });

            modelBuilder.Entity<ThongBao>()
                .HasIndex(t => t.ThoiGianTao);

            // Cấu hình quan hệ cho ChatMessage
            modelBuilder.Entity<ChatMessage>()
                .HasOne(c => c.NguoiGui)
                .WithMany()
                .HasForeignKey(c => c.NguoiGuiId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChatMessage>()
                .HasOne(c => c.NguoiNhan)
                .WithMany()
                .HasForeignKey(c => c.NguoiNhanId)
                .OnDelete(DeleteBehavior.SetNull);

            // Index cho ChatMessage
            modelBuilder.Entity<ChatMessage>()
                .HasIndex(c => new { c.NguoiGuiId, c.NguoiNhanId, c.ThoiGianGui });

            modelBuilder.Entity<ChatMessage>()
                .HasIndex(c => new { c.NguoiNhanId, c.DaDoc });
        }
    }
}
