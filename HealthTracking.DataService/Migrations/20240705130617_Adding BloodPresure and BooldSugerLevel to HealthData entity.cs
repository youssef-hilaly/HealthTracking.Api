using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthTracking.DataService.Migrations
{
    /// <inheritdoc />
    public partial class AddingBloodPresureandBooldSugerLeveltoHealthDataentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BloodPresure",
                table: "HealthData",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BooldSugerLevel",
                table: "HealthData",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BloodPresure",
                table: "HealthData");

            migrationBuilder.DropColumn(
                name: "BooldSugerLevel",
                table: "HealthData");
        }
    }
}
