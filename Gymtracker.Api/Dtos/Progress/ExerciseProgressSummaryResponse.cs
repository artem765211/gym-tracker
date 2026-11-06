namespace Gymtracker.Api.Dtos.Progress;

// Для карточек: последний месяц с записями по упражнению.
public record ExerciseProgressSummaryResponse(
    int ExerciseId,
    string ExerciseName,
    string MuscleGroup,
    MonthlyProgressResponse LatestMonth);
