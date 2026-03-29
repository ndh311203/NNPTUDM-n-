-- Script SQL để thêm các cột mới vào bảng HoaDons nếu migration chưa được apply
-- Chạy script này trong SQL Server Management Studio hoặc Azure Data Studio

USE spatc; -- Thay đổi tên database nếu cần
GO

-- Kiểm tra và thêm cột KhachHangEmail nếu chưa có
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'HoaDons' AND COLUMN_NAME = 'KhachHangEmail')
BEGIN
    ALTER TABLE HoaDons
    ADD KhachHangEmail NVARCHAR(100) NULL;
    PRINT 'Đã thêm cột KhachHangEmail';
END
ELSE
    PRINT 'Cột KhachHangEmail đã tồn tại';
GO

-- Kiểm tra và thêm cột KhachHangSoDienThoai nếu chưa có
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'HoaDons' AND COLUMN_NAME = 'KhachHangSoDienThoai')
BEGIN
    ALTER TABLE HoaDons
    ADD KhachHangSoDienThoai NVARCHAR(20) NULL;
    PRINT 'Đã thêm cột KhachHangSoDienThoai';
END
ELSE
    PRINT 'Cột KhachHangSoDienThoai đã tồn tại';
GO

-- Kiểm tra và thêm cột KhachHangTen nếu chưa có
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'HoaDons' AND COLUMN_NAME = 'KhachHangTen')
BEGIN
    ALTER TABLE HoaDons
    ADD KhachHangTen NVARCHAR(100) NULL;
    PRINT 'Đã thêm cột KhachHangTen';
END
ELSE
    PRINT 'Cột KhachHangTen đã tồn tại';
GO

-- Kiểm tra và thêm cột LoaiDichVu nếu chưa có
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'HoaDons' AND COLUMN_NAME = 'LoaiDichVu')
BEGIN
    ALTER TABLE HoaDons
    ADD LoaiDichVu NVARCHAR(50) NULL;
    PRINT 'Đã thêm cột LoaiDichVu';
END
ELSE
    PRINT 'Cột LoaiDichVu đã tồn tại';
GO

-- Kiểm tra và thêm cột LoaiHoaDon nếu chưa có
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'HoaDons' AND COLUMN_NAME = 'LoaiHoaDon')
BEGIN
    ALTER TABLE HoaDons
    ADD LoaiHoaDon NVARCHAR(50) NOT NULL DEFAULT 'TuDatLich';
    PRINT 'Đã thêm cột LoaiHoaDon';
END
ELSE
    PRINT 'Cột LoaiHoaDon đã tồn tại';
GO

-- Kiểm tra và thêm cột TenDichVu nếu chưa có
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'HoaDons' AND COLUMN_NAME = 'TenDichVu')
BEGIN
    ALTER TABLE HoaDons
    ADD TenDichVu NVARCHAR(200) NULL;
    PRINT 'Đã thêm cột TenDichVu';
END
ELSE
    PRINT 'Cột TenDichVu đã tồn tại';
GO

-- Kiểm tra và sửa DatLichId thành nullable nếu chưa
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
          WHERE TABLE_NAME = 'HoaDons' AND COLUMN_NAME = 'DatLichId' AND IS_NULLABLE = 'NO')
BEGIN
    -- Xóa foreign key constraint trước
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_HoaDons_DatLiches_DatLichId')
    BEGIN
        ALTER TABLE HoaDons
        DROP CONSTRAINT FK_HoaDons_DatLiches_DatLichId;
        PRINT 'Đã xóa foreign key constraint cũ';
    END
    
    -- Xóa index nếu có
    IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_HoaDons_DatLichId')
    BEGIN
        DROP INDEX IX_HoaDons_DatLichId ON HoaDons;
        PRINT 'Đã xóa index cũ';
    END
    
    -- Sửa cột thành nullable
    ALTER TABLE HoaDons
    ALTER COLUMN DatLichId INT NULL;
    PRINT 'Đã sửa DatLichId thành nullable';
    
    -- Tạo lại index với filter
    CREATE UNIQUE NONCLUSTERED INDEX IX_HoaDons_DatLichId
    ON HoaDons(DatLichId)
    WHERE DatLichId IS NOT NULL;
    PRINT 'Đã tạo lại index với filter';
    
    -- Tạo lại foreign key với SetNull
    ALTER TABLE HoaDons
    ADD CONSTRAINT FK_HoaDons_DatLiches_DatLichId
    FOREIGN KEY (DatLichId) REFERENCES DatLiches(Id)
    ON DELETE SET NULL;
    PRINT 'Đã tạo lại foreign key với SetNull';
END
ELSE
    PRINT 'DatLichId đã là nullable';
GO

PRINT 'Hoàn thành!';
GO














