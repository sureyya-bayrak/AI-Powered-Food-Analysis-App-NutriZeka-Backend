using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriZeka.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSourceToScanHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "ScanHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Source",
                table: "ScanHistories");
        }
    }
}
