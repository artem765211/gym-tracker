using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gymtracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveWorkingRepsFromWorkoutExercise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkingReps",
                table: "WorkoutExercises");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkingReps",
                table: "WorkoutExercises",
                type: "integer",
                nullable: true);
        }
    }
}
