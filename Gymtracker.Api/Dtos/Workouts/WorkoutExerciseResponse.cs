namespace Gymtracker.Api.Dtos.Workouts;

public record WorkoutExerciseResponse(
    int ExerciseId,
    string Name,
    string MuscleGroup,
    decimal? WorkingWeight);
