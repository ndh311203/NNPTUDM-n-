using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace spatc.Migrations
{
    /// <inheritdoc />
    public partial class AddGroomingAndKhachSanModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DichVuId",
                table: "DatLiches",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "DichVuGroomingId",
                table: "DatLiches",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GhiChuDacBiet",
                table: "DatLiches",
                type: "ntext",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PhongKhachSanId",
                table: "DatLiches",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ThoiGianKetThuc",
                table: "DatLiches",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DichVuGroomings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDichVu = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MoTa = table.Column<string>(type: "ntext", nullable: true),
                    Gia = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ThoiGianThucHien = table.Column<int>(type: "int", nullable: true),
                    HinhAnh = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DichVuGroomings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhongKhachSans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenPhong = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LoaiPhong = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GiaTheoGio = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GiaTheoNgay = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SoLuongToiDa = table.Column<int>(type: "int", nullable: false),
                    TienNghi = table.Column<string>(type: "ntext", nullable: true),
                    HinhAnh = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhongKhachSans", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DatLiches_DichVuGroomingId",
                table: "DatLiches",
                column: "DichVuGroomingId");

            migrationBuilder.CreateIndex(
                name: "IX_DatLiches_PhongKhachSanId",
                table: "DatLiches",
                column: "PhongKhachSanId");

            migrationBuilder.AddForeignKey(
                name: "FK_DatLiches_DichVuGroomings_DichVuGroomingId",
                table: "DatLiches",
                column: "DichVuGroomingId",
                principalTable: "DichVuGroomings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DatLiches_PhongKhachSans_PhongKhachSanId",
                table: "DatLiches",
                column: "PhongKhachSanId",
                principalTable: "PhongKhachSans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DatLiches_DichVuGroomings_DichVuGroomingId",
                table: "DatLiches");

            migrationBuilder.DropForeignKey(
                name: "FK_DatLiches_PhongKhachSans_PhongKhachSanId",
                table: "DatLiches");

            migrationBuilder.DropTable(
                name: "DichVuGroomings");

            migrationBuilder.DropTable(
                name: "PhongKhachSans");

            migrationBuilder.DropIndex(
                name: "IX_DatLiches_DichVuGroomingId",
                table: "DatLiches");

            migrationBuilder.DropIndex(
                name: "IX_DatLiches_PhongKhachSanId",
                table: "DatLiches");

            migrationBuilder.DropColumn(
                name: "DichVuGroomingId",
                table: "DatLiches");

            migrationBuilder.DropColumn(
                name: "GhiChuDacBiet",
                table: "DatLiches");

            migrationBuilder.DropColumn(
                name: "PhongKhachSanId",
                table: "DatLiches");

            migrationBuilder.DropColumn(
                name: "ThoiGianKetThuc",
                table: "DatLiches");

            migrationBuilder.AlterColumn<int>(
                name: "DichVuId",
                table: "DatLiches",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
