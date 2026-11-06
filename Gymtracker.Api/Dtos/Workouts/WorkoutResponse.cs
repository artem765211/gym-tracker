namespace Gymtracker.Api.Dtos.Workouts;

public record WorkoutResponse(
    int Id,
    DateOnly Date,
    string? Note,
    List<WorkoutExerciseResponse> Exercises);
