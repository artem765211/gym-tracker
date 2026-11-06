using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gymtracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkoutUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Workouts_UserId_Date",
                table: "Workouts");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutExercises_WorkoutId",
                table: "WorkoutExercises");

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_UserId_Date",
                table: "Workouts",
                columns: new[] { "UserId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutExercises_WorkoutId_ExerciseId",
                table: "WorkoutExercises",
                columns: new[] { "WorkoutId", "ExerciseId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Workouts_UserId_Date",
                table: "Workouts");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutExercises_WorkoutId_ExerciseId",
                table: "WorkoutExercises");

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_UserId_Date",
                table: "Workouts",
                columns: new[] { "UserId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutExercises_WorkoutId",
                table: "WorkoutExercises",
                column: "WorkoutId");
        }
    }
}
