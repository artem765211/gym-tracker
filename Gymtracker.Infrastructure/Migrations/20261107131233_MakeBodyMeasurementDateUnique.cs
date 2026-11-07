using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gymtracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeBodyMeasurementDateUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BodyMeasurements_UserId_Date",
                table: "BodyMeasurements");

            migrationBuilder.CreateIndex(
                name: "IX_BodyMeasurements_UserId_Date",
                table: "BodyMeasurements",
                columns: new[] { "UserId", "Date" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BodyMeasurements_UserId_Date",
                table: "BodyMeasurements");

            migrationBuilder.CreateIndex(
                name: "IX_BodyMeasurements_UserId_Date",
                table: "BodyMeasurements",
                columns: new[] { "UserId", "Date" });
        }
    }
}
