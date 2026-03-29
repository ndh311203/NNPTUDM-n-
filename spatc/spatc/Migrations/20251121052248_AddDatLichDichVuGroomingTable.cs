using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace spatc.Migrations
{
    /// <inheritdoc />
    public partial class AddDatLichDichVuGroomingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DatLichDichVuGroomings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatLichId = table.Column<int>(type: "int", nullable: false),
                    DichVuGroomingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatLichDichVuGroomings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DatLichDichVuGroomings_DatLiches_DatLichId",
                        column: x => x.DatLichId,
                        principalTable: "DatLiches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DatLichDichVuGroomings_DichVuGroomings_DichVuGroomingId",
                        column: x => x.DichVuGroomingId,
                        principalTable: "DichVuGroomings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DatLichDichVuGroomings_DatLichId",
                table: "DatLichDichVuGroomings",
                column: "DatLichId");

            migrationBuilder.CreateIndex(
                name: "IX_DatLichDichVuGroomings_DichVuGroomingId",
                table: "DatLichDichVuGroomings",
                column: "DichVuGroomingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DatLichDichVuGroomings");
        }
    }
}
