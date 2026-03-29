using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace spatc.Migrations
{
    /// <inheritdoc />
    public partial class AddChatMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NguoiGuiId = table.Column<int>(type: "int", nullable: false),
                    NguoiNhanId = table.Column<int>(type: "int", nullable: true),
                    NoiDung = table.Column<string>(type: "ntext", nullable: false),
                    LoaiTinNhan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DaDoc = table.Column<bool>(type: "bit", nullable: false),
                    ThoiGianGui = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_TaiKhoans_NguoiGuiId",
                        column: x => x.NguoiGuiId,
                        principalTable: "TaiKhoans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChatMessages_TaiKhoans_NguoiNhanId",
                        column: x => x.NguoiNhanId,
                        principalTable: "TaiKhoans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_NguoiGuiId_NguoiNhanId_ThoiGianGui",
                table: "ChatMessages",
                columns: new[] { "NguoiGuiId", "NguoiNhanId", "ThoiGianGui" });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_NguoiNhanId_DaDoc",
                table: "ChatMessages",
                columns: new[] { "NguoiNhanId", "DaDoc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMessages");
        }
    }
}
