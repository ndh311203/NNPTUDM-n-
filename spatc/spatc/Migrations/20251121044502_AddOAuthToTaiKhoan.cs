using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace spatc.Migrations
{
    /// <inheritdoc />
    public partial class AddOAuthToTaiKhoan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Provider",
                table: "TaiKhoans",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderId",
                table: "TaiKhoans",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Provider",
                table: "TaiKhoans");

            migrationBuilder.DropColumn(
                name: "ProviderId",
                table: "TaiKhoans");
        }
    }
}
