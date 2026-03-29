using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace spatc.Migrations
{
    /// <inheritdoc />
    public partial class AddThongBaoTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThongBaos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NguoiNhanId = table.Column<int>(type: "int", nullable: true),
                    LoaiThongBao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TieuDe = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NoiDung = table.Column<string>(type: "ntext", nullable: false),
                    DatLichId = table.Column<int>(type: "int", nullable: true),
                    ThuCungId = table.Column<int>(type: "int", nullable: true),
                    DaDoc = table.Column<bool>(type: "bit", nullable: false),
                    ThoiGianTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongBaos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThongBaos_DatLiches_DatLichId",
                        column: x => x.DatLichId,
                        principalTable: "DatLiches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ThongBaos_TaiKhoans_NguoiNhanId",
                        column: x => x.NguoiNhanId,
                        principalTable: "TaiKhoans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ThongBaos_ThuCungs_ThuCungId",
                        column: x => x.ThuCungId,
                        principalTable: "ThuCungs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaos_DatLichId",
                table: "ThongBaos",
                column: "DatLichId");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaos_NguoiNhanId_DaDoc",
                table: "ThongBaos",
                columns: new[] { "NguoiNhanId", "DaDoc" });

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaos_ThoiGianTao",
                table: "ThongBaos",
                column: "ThoiGianTao");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaos_ThuCungId",
                table: "ThongBaos",
                column: "ThuCungId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThongBaos");
        }
    }
}
