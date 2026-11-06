namespace Gymtracker.Api.Dtos.Exercises;

// IsCustom: true = личное упражнение пользователя, false = общее из справочника
public record ExerciseResponse(int Id, string Name, string MuscleGroup, bool IsCustom);
