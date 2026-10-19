namespace Gymtracker.Core.Entities;

public class Workout
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public string? Note { get; set; }

    public List<WorkoutExercise> Exercises { get; set; } = new();
}
