using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace spatc.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePetShopModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PetProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "FoodProducts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPercent",
                table: "FoodProducts",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "GalleryImages",
                table: "FoodProducts",
                type: "ntext",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "FoodProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SKU",
                table: "FoodProducts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPercent",
                table: "AccessoryProducts",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "GalleryImages",
                table: "AccessoryProducts",
                type: "ntext",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AccessoryProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ProductCategoryId",
                table: "AccessoryProducts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SKU",
                table: "AccessoryProducts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "ntext", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FoodProducts_CategoryId",
                table: "FoodProducts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodProducts_SKU",
                table: "FoodProducts",
                column: "SKU",
                unique: true,
                filter: "[SKU] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AccessoryProducts_ProductCategoryId",
                table: "AccessoryProducts",
                column: "ProductCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessoryProducts_SKU",
                table: "AccessoryProducts",
                column: "SKU",
                unique: true,
                filter: "[SKU] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_Slug",
                table: "ProductCategories",
                column: "Slug",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AccessoryProducts_ProductCategories_ProductCategoryId",
                table: "AccessoryProducts",
                column: "ProductCategoryId",
                principalTable: "ProductCategories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FoodProducts_ProductCategories_CategoryId",
                table: "FoodProducts",
                column: "CategoryId",
                principalTable: "ProductCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessoryProducts_ProductCategories_ProductCategoryId",
                table: "AccessoryProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_FoodProducts_ProductCategories_CategoryId",
                table: "FoodProducts");

            migrationBuilder.DropTable(
                name: "ProductCategories");

            migrationBuilder.DropIndex(
                name: "IX_FoodProducts_CategoryId",
                table: "FoodProducts");

            migrationBuilder.DropIndex(
                name: "IX_FoodProducts_SKU",
                table: "FoodProducts");

            migrationBuilder.DropIndex(
                name: "IX_AccessoryProducts_ProductCategoryId",
                table: "AccessoryProducts");

            migrationBuilder.DropIndex(
                name: "IX_AccessoryProducts_SKU",
                table: "AccessoryProducts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PetProducts");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "FoodProducts");

            migrationBuilder.DropColumn(
                name: "DiscountPercent",
                table: "FoodProducts");

            migrationBuilder.DropColumn(
                name: "GalleryImages",
                table: "FoodProducts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "FoodProducts");

            migrationBuilder.DropColumn(
                name: "SKU",
                table: "FoodProducts");

            migrationBuilder.DropColumn(
                name: "DiscountPercent",
                table: "AccessoryProducts");

            migrationBuilder.DropColumn(
                name: "GalleryImages",
                table: "AccessoryProducts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AccessoryProducts");

            migrationBuilder.DropColumn(
                name: "ProductCategoryId",
                table: "AccessoryProducts");

            migrationBuilder.DropColumn(
                name: "SKU",
                table: "AccessoryProducts");
        }
    }
}
