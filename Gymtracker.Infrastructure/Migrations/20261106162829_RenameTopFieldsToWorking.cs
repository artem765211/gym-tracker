using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gymtracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameTopFieldsToWorking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TopWeight",
                table: "WorkoutExercises",
                newName: "WorkingWeight");

            migrationBuilder.RenameColumn(
                name: "TopReps",
                table: "WorkoutExercises",
                newName: "WorkingReps");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WorkingWeight",
                table: "WorkoutExercises",
                newName: "TopWeight");

            migrationBuilder.RenameColumn(
                name: "WorkingReps",
                table: "WorkoutExercises",
                newName: "TopReps");
        }
    }
}
