using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace spatc.Migrations
{
    /// <inheritdoc />
    public partial class RenameTaiKhoanldToTaiKhoanId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Kiểm tra và đổi tên cột nếu cột sai tên tồn tại
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                           WHERE TABLE_NAME = 'KhachHangs' AND COLUMN_NAME = 'TaiKhoanld')
                BEGIN
                    EXEC sp_rename 'KhachHangs.TaiKhoanld', 'TaiKhoanId', 'COLUMN';
                END
            ");

            // Nếu cột chưa tồn tại, thêm mới
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                               WHERE TABLE_NAME = 'KhachHangs' AND COLUMN_NAME = 'TaiKhoanId')
                BEGIN
                    ALTER TABLE [KhachHangs] ADD [TaiKhoanId] int NULL;
                END
            ");

            // Tạo index nếu chưa tồn tại
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes 
                               WHERE name = 'IX_KhachHangs_TaiKhoanId' AND object_id = OBJECT_ID('KhachHangs'))
                BEGIN
                    CREATE UNIQUE INDEX [IX_KhachHangs_TaiKhoanId] 
                    ON [KhachHangs] ([TaiKhoanId]) 
                    WHERE [TaiKhoanId] IS NOT NULL;
                END
            ");

            // Thêm foreign key nếu chưa tồn tại
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys 
                               WHERE name = 'FK_KhachHangs_TaiKhoans_TaiKhoanId')
                BEGIN
                    ALTER TABLE [KhachHangs] 
                    ADD CONSTRAINT [FK_KhachHangs_TaiKhoans_TaiKhoanId] 
                    FOREIGN KEY ([TaiKhoanId]) 
                    REFERENCES [TaiKhoans] ([Id]) 
                    ON DELETE SET NULL;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.foreign_keys 
                           WHERE name = 'FK_KhachHangs_TaiKhoans_TaiKhoanId')
                BEGIN
                    ALTER TABLE [KhachHangs] 
                    DROP CONSTRAINT [FK_KhachHangs_TaiKhoans_TaiKhoanId];
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.indexes 
                           WHERE name = 'IX_KhachHangs_TaiKhoanId' AND object_id = OBJECT_ID('KhachHangs'))
                BEGIN
                    DROP INDEX [IX_KhachHangs_TaiKhoanId] ON [KhachHangs];
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                           WHERE TABLE_NAME = 'KhachHangs' AND COLUMN_NAME = 'TaiKhoanId')
                BEGIN
                    ALTER TABLE [KhachHangs] DROP COLUMN [TaiKhoanId];
                END
            ");
        }
    }
}
