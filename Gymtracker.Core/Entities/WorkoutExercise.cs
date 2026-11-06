namespace Gymtracker.Core.Entities;

public class WorkoutExercise
{
    public int Id { get; set; }

    public int WorkoutId { get; set; }
    public Workout Workout { get; set; } = null!;

    public int ExerciseId { get; set; }
    public Exercise Exercise { get; set; } = null!;

    // Рабочий вес в этой тренировке, необязательное поле. На расчёт прироста не влияет.
    public decimal? WorkingWeight { get; set; }
}
