using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace spatc.Migrations
{
    /// <inheritdoc />
    public partial class AddLoaiDichVuAndSanPham : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LoaiDichVu",
                table: "DichVus",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoaiDichVu",
                table: "DichVus");
        }
    }
}
