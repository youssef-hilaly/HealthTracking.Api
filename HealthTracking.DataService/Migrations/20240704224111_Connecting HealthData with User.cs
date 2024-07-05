using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthTracking.DataService.Migrations
{
    /// <inheritdoc />
    public partial class ConnectingHealthDatawithUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "userId",
                table: "HealthData",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_HealthData_userId",
                table: "HealthData",
                column: "userId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_HealthData_Users_userId",
                table: "HealthData",
                column: "userId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HealthData_Users_userId",
                table: "HealthData");

            migrationBuilder.DropIndex(
                name: "IX_HealthData_userId",
                table: "HealthData");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "HealthData");
        }
    }
}
