namespace Gymtracker.Core.Entities;

public class WorkoutExercise
{
    public int Id { get; set; }

    public int WorkoutId { get; set; }
    public Workout Workout { get; set; } = null!;

    public int ExerciseId { get; set; }
    public Exercise Exercise { get; set; } = null!;

    // Лучший подход, вносится на вкладке «Сила». Необязательные поля.
    public decimal? TopWeight { get; set; }
    public int? TopReps { get; set; }
}
