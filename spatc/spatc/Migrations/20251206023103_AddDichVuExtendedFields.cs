using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace spatc.Migrations
{
    /// <inheritdoc />
    public partial class AddDichVuExtendedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BaoGomBuaAn",
                table: "DichVus",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DichVuBoSung",
                table: "DichVus",
                type: "ntext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KichThuocThuCung",
                table: "DichVus",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoaiDichVuGrooming",
                table: "DichVus",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoaiPhong",
                table: "DichVus",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SucChua",
                table: "DichVus",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThoiGianNhanTra",
                table: "DichVus",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ThoiGianThucHien",
                table: "DichVus",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaoGomBuaAn",
                table: "DichVus");

            migrationBuilder.DropColumn(
                name: "DichVuBoSung",
                table: "DichVus");

            migrationBuilder.DropColumn(
                name: "KichThuocThuCung",
                table: "DichVus");

            migrationBuilder.DropColumn(
                name: "LoaiDichVuGrooming",
                table: "DichVus");

            migrationBuilder.DropColumn(
                name: "LoaiPhong",
                table: "DichVus");

            migrationBuilder.DropColumn(
                name: "SucChua",
                table: "DichVus");

            migrationBuilder.DropColumn(
                name: "ThoiGianNhanTra",
                table: "DichVus");

            migrationBuilder.DropColumn(
                name: "ThoiGianThucHien",
                table: "DichVus");
        }
    }
}
