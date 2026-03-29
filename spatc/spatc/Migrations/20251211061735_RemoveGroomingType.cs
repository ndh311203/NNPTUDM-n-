using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace spatc.Migrations
{
    /// <inheritdoc />
    public partial class RemoveGroomingType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DichVuGroomings_GroomingTypes_GroomingTypeId",
                table: "DichVuGroomings");

            migrationBuilder.DropForeignKey(
                name: "FK_DichVus_GroomingTypes_GroomingTypeId",
                table: "DichVus");

            migrationBuilder.DropTable(
                name: "GroomingTypes");

            migrationBuilder.DropIndex(
                name: "IX_DichVus_GroomingTypeId",
                table: "DichVus");

            migrationBuilder.DropIndex(
                name: "IX_DichVuGroomings_GroomingTypeId",
                table: "DichVuGroomings");

            migrationBuilder.DropColumn(
                name: "GroomingTypeId",
                table: "DichVus");

            migrationBuilder.DropColumn(
                name: "GroomingTypeId",
                table: "DichVuGroomings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroomingTypeId",
                table: "DichVus",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GroomingTypeId",
                table: "DichVuGroomings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GroomingTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenLoai = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroomingTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DichVus_GroomingTypeId",
                table: "DichVus",
                column: "GroomingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DichVuGroomings_GroomingTypeId",
                table: "DichVuGroomings",
                column: "GroomingTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_DichVuGroomings_GroomingTypes_GroomingTypeId",
                table: "DichVuGroomings",
                column: "GroomingTypeId",
                principalTable: "GroomingTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_DichVus_GroomingTypes_GroomingTypeId",
                table: "DichVus",
                column: "GroomingTypeId",
                principalTable: "GroomingTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
