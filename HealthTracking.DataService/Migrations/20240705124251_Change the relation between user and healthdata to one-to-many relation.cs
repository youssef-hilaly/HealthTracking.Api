using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthTracking.DataService.Migrations
{
    /// <inheritdoc />
    public partial class Changetherelationbetweenuserandhealthdatatoonetomanyrelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HealthData_userId",
                table: "HealthData");

            migrationBuilder.CreateIndex(
                name: "IX_HealthData_userId",
                table: "HealthData",
                column: "userId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HealthData_userId",
                table: "HealthData");

            migrationBuilder.CreateIndex(
                name: "IX_HealthData_userId",
                table: "HealthData",
                column: "userId",
                unique: true);
        }
    }
}
