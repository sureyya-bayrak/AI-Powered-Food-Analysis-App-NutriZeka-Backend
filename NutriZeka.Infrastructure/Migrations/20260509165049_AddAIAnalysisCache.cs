using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriZeka.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAIAnalysisCache : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AIAnalysisCaches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScanHistoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionType = table.Column<int>(type: "int", nullable: false),
                    UserWasGlutenFree = table.Column<bool>(type: "bit", nullable: false),
                    UserWasLactoseFree = table.Column<bool>(type: "bit", nullable: false),
                    UserWasPalmOilFree = table.Column<bool>(type: "bit", nullable: false),
                    AIResponse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIAnalysisCaches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AIAnalysisCaches_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AIAnalysisCaches_ScanHistories_ScanHistoryId",
                        column: x => x.ScanHistoryId,
                        principalTable: "ScanHistories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AIAnalysisCaches_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AIAnalysisCaches_ProductId",
                table: "AIAnalysisCaches",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_AIAnalysisCaches_ScanHistoryId",
                table: "AIAnalysisCaches",
                column: "ScanHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AIAnalysisCaches_UserId",
                table: "AIAnalysisCaches",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AIAnalysisCaches");
        }
    }
}
