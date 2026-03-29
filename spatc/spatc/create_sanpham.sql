IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251118062108_InitialSpaDatabase'
)
BEGIN
    CREATE TABLE [BaiViets] (
        [Id] int NOT NULL IDENTITY,
        [TieuDe] nvarchar(200) NOT NULL,
        [MoTaNgan] nvarchar(500) NULL,
        [NoiDung] ntext NULL,
        [HinhAnh] nvarchar(500) NULL,
        [NgayDang] datetime2 NOT NULL,
        [LuotXem] int NOT NULL,
        [TrangThai] bit NOT NULL,
        CONSTRAINT [PK_BaiViets] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251118062108_InitialSpaDatabase'
)
BEGIN
    CREATE TABLE [DichVus] (
        [Id] int NOT NULL IDENTITY,
        [TenDichVu] nvarchar(200) NOT NULL,
        [MoTa] ntext NULL,
        [Gia] decimal(18,2) NOT NULL,
        [HinhAnh] nvarchar(500) NULL,
        [NgayTao] datetime2 NOT NULL,
        [TrangThai] bit NOT NULL,
        CONSTRAINT [PK_DichVus] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251118062108_InitialSpaDatabase'
)
BEGIN
    CREATE TABLE [KhachHangs] (
        [Id] int NOT NULL IDENTITY,
        [HoTen] nvarchar(100) NOT NULL,
        [SoDienThoai] nvarchar(20) NOT NULL,
        [Email] nvarchar(100) NULL,
        [DiaChi] nvarchar(500) NULL,
        [NgayTao] datetime2 NOT NULL,
        [GhiChu] nvarchar(1000) NULL,
        CONSTRAINT [PK_KhachHangs] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251118062108_InitialSpaDatabase'
)
BEGIN
    CREATE TABLE [NhanViens] (
        [Id] int NOT NULL IDENTITY,
        [HoTen] nvarchar(100) NOT NULL,
        [ViTri] nvarchar(100) NOT NULL,
        [SoDienThoai] nvarchar(20) NULL,
        [Email] nvarchar(100) NULL,
        [AnhNhanVien] nvarchar(500) NULL,
        [LichLamViec] nvarchar(500) NULL,
        [NgayTao] datetime2 NOT NULL,
        [TrangThai] bit NOT NULL,
        CONSTRAINT [PK_NhanViens] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251118062108_InitialSpaDatabase'
)
BEGIN
    CREATE TABLE [ThuCungs] (
        [Id] int NOT NULL IDENTITY,
        [TenThuCung] nvarchar(100) NOT NULL,
        [Loai] nvarchar(50) NOT NULL,
        [Giong] nvarchar(100) NULL,
        [Tuoi] int NULL,
        [CanNang] decimal(5,2) NULL,
        [GhiChuSucKhoe] ntext NULL,
        [HinhAnh] nvarchar(500) NULL,
        [NgayTao] datetime2 NOT NULL,
        [KhachHangId] int NOT NULL,
        CONSTRAINT [PK_ThuCungs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ThuCungs_KhachHangs_KhachHangId] FOREIGN KEY ([KhachHangId]) REFERENCES [KhachHangs] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251118062108_InitialSpaDatabase'
)
BEGIN
    CREATE TABLE [DatLiches] (
        [Id] int NOT NULL IDENTITY,
        [ThoiGianHen] datetime2 NOT NULL,
        [GhiChu] ntext NULL,
        [TrangThai] nvarchar(50) NOT NULL,
        [NgayTao] datetime2 NOT NULL,
        [ThuCungId] int NOT NULL,
        [DichVuId] int NOT NULL,
        [NhanVienId] int NULL,
        CONSTRAINT [PK_DatLiches] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DatLiches_DichVus_DichVuId] FOREIGN KEY ([DichVuId]) REFERENCES [DichVus] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_DatLiches_NhanViens_NhanVienId] FOREIGN KEY ([NhanVienId]) REFERENCES [NhanViens] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_DatLiches_ThuCungs_ThuCungId] FOREIGN KEY ([ThuCungId]) REFERENCES [ThuCungs] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251118062108_InitialSpaDatabase'
)
BEGIN
    CREATE TABLE [HoaDons] (
        [Id] int NOT NULL IDENTITY,
        [NgayThanhToan] datetime2 NOT NULL,
        [TongTien] decimal(18,2) NOT NULL,
        [PhuongThucThanhToan] nvarchar(50) NULL,
        [GhiChu] nvarchar(500) NULL,
        [NgayTao] datetime2 NOT NULL,
        [DatLichId] int NOT NULL,
        CONSTRAINT [PK_HoaDons] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HoaDons_DatLiches_DatLichId] FOREIGN KEY ([DatLichId]) REFERENCES [DatLiches] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251118062108_InitialSpaDatabase'
)
BEGIN
    CREATE INDEX [IX_DatLiches_DichVuId] ON [DatLiches] ([DichVuId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251118062108_InitialSpaDatabase'
)
BEGIN
    CREATE INDEX [IX_DatLiches_NhanVienId] ON [DatLiches] ([NhanVienId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251118062108_InitialSpaDatabase'
)
BEGIN
    CREATE INDEX [IX_DatLiches_ThoiGianHen] ON [DatLiches] ([ThoiGianHen]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251118062108_InitialSpaDatabase'
)
BEGIN
    CREATE INDEX [IX_DatLiches_ThuCungId] ON [DatLiches] ([ThuCungId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251118062108_InitialSpaDatabase'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HoaDons_DatLichId] ON [HoaDons] ([DatLichId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251118062108_InitialSpaDatabase'
)
BEGIN
    CREATE UNIQUE INDEX [IX_KhachHangs_SoDienThoai] ON [KhachHangs] ([SoDienThoai]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251118062108_InitialSpaDatabase'
)
BEGIN
    CREATE INDEX [IX_ThuCungs_KhachHangId] ON [ThuCungs] ([KhachHangId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251118062108_InitialSpaDatabase'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251118062108_InitialSpaDatabase', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251118064901_AddTaiKhoanTable'
)
BEGIN
    CREATE TABLE [TaiKhoans] (
        [Id] int NOT NULL IDENTITY,
        [Email] nvarchar(100) NOT NULL,
        [MatKhau] nvarchar(255) NOT NULL,
        [HoTen] nvarchar(100) NULL,
        [VaiTro] nvarchar(50) NOT NULL,
        [NgayTao] datetime2 NOT NULL,
        [TrangThai] bit NOT NULL,
        CONSTRAINT [PK_TaiKhoans] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251118064901_AddTaiKhoanTable'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TaiKhoans_Email] ON [TaiKhoans] ([Email]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251118064901_AddTaiKhoanTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251118064901_AddTaiKhoanTable', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251118073107_RemoveBaiVietTable'
)
BEGIN
    DROP TABLE [BaiViets];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251118073107_RemoveBaiVietTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251118073107_RemoveBaiVietTable', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251121043337_AddLoaiDichVuAndSanPham'
)
BEGIN
    ALTER TABLE [DichVus] ADD [LoaiDichVu] nvarchar(50) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251121043337_AddLoaiDichVuAndSanPham'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251121043337_AddLoaiDichVuAndSanPham', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251121043604_AddSanPhamTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251121043604_AddSanPhamTable', N'8.0.0');
END;
GO

COMMIT;
GO

