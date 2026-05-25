using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriZeka.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAiLimits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DailyAiRequestCount",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAiRequestDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DailyAiRequestCount",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastAiRequestDate",
                table: "Users");
        }
    }
}
