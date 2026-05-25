using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriZeka.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductNewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Categories",
                table: "Products",
                newName: "CategoriesTr");

            migrationBuilder.AddColumn<string>(
                name: "CategoriesEn",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "ContainsGluten",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ContainsLactose",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ContainsPalmOil",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoriesEn",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ContainsGluten",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ContainsLactose",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ContainsPalmOil",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "CategoriesTr",
                table: "Products",
                newName: "Categories");
        }
    }
}
