using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace spatc.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHoaDonForWalkInCustomers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_HoaDons_DatLiches_DatLichId' AND parent_object_id = OBJECT_ID('HoaDons'))
                BEGIN
                    ALTER TABLE HoaDons DROP CONSTRAINT FK_HoaDons_DatLiches_DatLichId;
                END
            ");

            // Drop index if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_HoaDons_DatLichId' AND object_id = OBJECT_ID('HoaDons'))
                BEGIN
                    DROP INDEX IX_HoaDons_DatLichId ON HoaDons;
                END
            ");

            // Alter column to nullable only if it's not already nullable
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                           WHERE TABLE_NAME = 'HoaDons' AND COLUMN_NAME = 'DatLichId' AND IS_NULLABLE = 'NO')
                BEGIN
                    DECLARE @var0 sysname;
                    SELECT @var0 = [d].[name]
                    FROM [sys].[default_constraints] [d]
                    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
                    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[HoaDons]') AND [c].[name] = N'DatLichId');
                    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [HoaDons] DROP CONSTRAINT [' + @var0 + '];');
                    ALTER TABLE [HoaDons] ALTER COLUMN [DatLichId] int NULL;
                END
            ");

            // Add columns only if they don't exist (using raw SQL for idempotency)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                               WHERE TABLE_NAME = 'HoaDons' AND COLUMN_NAME = 'KhachHangEmail')
                BEGIN
                    ALTER TABLE HoaDons ADD KhachHangEmail NVARCHAR(100) NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                               WHERE TABLE_NAME = 'HoaDons' AND COLUMN_NAME = 'KhachHangSoDienThoai')
                BEGIN
                    ALTER TABLE HoaDons ADD KhachHangSoDienThoai NVARCHAR(20) NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                               WHERE TABLE_NAME = 'HoaDons' AND COLUMN_NAME = 'KhachHangTen')
                BEGIN
                    ALTER TABLE HoaDons ADD KhachHangTen NVARCHAR(100) NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                               WHERE TABLE_NAME = 'HoaDons' AND COLUMN_NAME = 'LoaiDichVu')
                BEGIN
                    ALTER TABLE HoaDons ADD LoaiDichVu NVARCHAR(50) NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                               WHERE TABLE_NAME = 'HoaDons' AND COLUMN_NAME = 'LoaiHoaDon')
                BEGIN
                    ALTER TABLE HoaDons ADD LoaiHoaDon NVARCHAR(50) NOT NULL DEFAULT 'TuDatLich';
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                               WHERE TABLE_NAME = 'HoaDons' AND COLUMN_NAME = 'TenDichVu')
                BEGIN
                    ALTER TABLE HoaDons ADD TenDichVu NVARCHAR(200) NULL;
                END
            ");

            // Create index if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_HoaDons_DatLichId' AND object_id = OBJECT_ID('HoaDons'))
                BEGIN
                    CREATE UNIQUE NONCLUSTERED INDEX IX_HoaDons_DatLichId
                    ON HoaDons(DatLichId)
                    WHERE DatLichId IS NOT NULL;
                END
            ");

            // Add foreign key if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_HoaDons_DatLiches_DatLichId' AND parent_object_id = OBJECT_ID('HoaDons'))
                BEGIN
                    ALTER TABLE HoaDons
                    ADD CONSTRAINT FK_HoaDons_DatLiches_DatLichId
                    FOREIGN KEY (DatLichId) REFERENCES DatLiches(Id)
                    ON DELETE SET NULL;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HoaDons_DatLiches_DatLichId",
                table: "HoaDons");

            migrationBuilder.DropIndex(
                name: "IX_HoaDons_DatLichId",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "KhachHangEmail",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "KhachHangSoDienThoai",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "KhachHangTen",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "LoaiDichVu",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "LoaiHoaDon",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "TenDichVu",
                table: "HoaDons");

            migrationBuilder.AlterColumn<int>(
                name: "DatLichId",
                table: "HoaDons",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_DatLichId",
                table: "HoaDons",
                column: "DatLichId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDons_DatLiches_DatLichId",
                table: "HoaDons",
                column: "DatLichId",
                principalTable: "DatLiches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
