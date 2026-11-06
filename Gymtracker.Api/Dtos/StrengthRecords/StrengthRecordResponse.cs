namespace Gymtracker.Api.Dtos.StrengthRecords;

public record StrengthRecordResponse(
    int Id,
    int ExerciseId,
    string ExerciseName,
    string MuscleGroup,
    DateOnly WeekStart,
    decimal Weight,
    int Reps);
