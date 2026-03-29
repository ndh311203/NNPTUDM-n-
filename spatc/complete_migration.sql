-- Script để hoàn thành migration 20251211072815_UpdateHoaDonForWalkInCustomers
-- Migration đã phần nào được apply (DatLichId đã nullable), nhưng các cột khác đã tồn tại

USE spatc; -- Thay đổi tên database nếu cần
GO

-- Kiểm tra và thêm các cột nếu chưa có (bỏ qua nếu đã tồn tại)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'HoaDons' AND COLUMN_NAME = 'KhachHangEmail')
BEGIN
    ALTER TABLE HoaDons ADD KhachHangEmail NVARCHAR(100) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'HoaDons' AND COLUMN_NAME = 'KhachHangSoDienThoai')
BEGIN
    ALTER TABLE HoaDons ADD KhachHangSoDienThoai NVARCHAR(20) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'HoaDons' AND COLUMN_NAME = 'KhachHangTen')
BEGIN
    ALTER TABLE HoaDons ADD KhachHangTen NVARCHAR(100) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'HoaDons' AND COLUMN_NAME = 'LoaiDichVu')
BEGIN
    ALTER TABLE HoaDons ADD LoaiDichVu NVARCHAR(50) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'HoaDons' AND COLUMN_NAME = 'LoaiHoaDon')
BEGIN
    ALTER TABLE HoaDons ADD LoaiHoaDon NVARCHAR(50) NOT NULL DEFAULT 'TuDatLich';
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'HoaDons' AND COLUMN_NAME = 'TenDichVu')
BEGIN
    ALTER TABLE HoaDons ADD TenDichVu NVARCHAR(200) NULL;
END
GO

-- Đảm bảo index được tạo đúng cách (với filter cho nullable)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_HoaDons_DatLichId' AND object_id = OBJECT_ID('HoaDons'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX IX_HoaDons_DatLichId
    ON HoaDons(DatLichId)
    WHERE DatLichId IS NOT NULL;
END
ELSE
BEGIN
    -- Kiểm tra xem index có filter không, nếu không thì xóa và tạo lại
    DECLARE @hasFilter BIT = 0;
    SELECT @hasFilter = CASE WHEN has_filter = 1 THEN 1 ELSE 0 END
    FROM sys.indexes 
    WHERE name = 'IX_HoaDons_DatLichId' AND object_id = OBJECT_ID('HoaDons');
    
    IF @hasFilter = 0
    BEGIN
        DROP INDEX IX_HoaDons_DatLichId ON HoaDons;
        CREATE UNIQUE NONCLUSTERED INDEX IX_HoaDons_DatLichId
        ON HoaDons(DatLichId)
        WHERE DatLichId IS NOT NULL;
    END
END
GO

-- Đảm bảo foreign key được tạo đúng cách
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_HoaDons_DatLiches_DatLichId' AND parent_object_id = OBJECT_ID('HoaDons'))
BEGIN
    ALTER TABLE HoaDons
    ADD CONSTRAINT FK_HoaDons_DatLiches_DatLichId
    FOREIGN KEY (DatLichId) REFERENCES DatLiches(Id)
    ON DELETE SET NULL;
END
GO

-- Đánh dấu migration đã được apply
IF NOT EXISTS (SELECT * FROM __EFMigrationsHistory WHERE MigrationId = '20251211072815_UpdateHoaDonForWalkInCustomers')
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20251211072815_UpdateHoaDonForWalkInCustomers', '8.0.0');
    PRINT 'Đã đánh dấu migration là đã apply';
END
ELSE
    PRINT 'Migration đã được đánh dấu là đã apply';
GO

PRINT 'Hoàn thành! Migration đã được apply thành công.';
GO

