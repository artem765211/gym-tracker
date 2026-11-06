namespace Gymtracker.Api.Dtos.Progress;

// Для графика и таблицы: месяцы идут от старых к новым.
public record ExerciseProgressResponse(
    int ExerciseId,
    string ExerciseName,
    string MuscleGroup,
    List<MonthlyProgressResponse> Months);
