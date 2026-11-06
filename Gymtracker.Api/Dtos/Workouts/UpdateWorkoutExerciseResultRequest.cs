using System.ComponentModel.DataAnnotations;

namespace Gymtracker.Api.Dtos.Workouts;

// Типы с ? нужны, чтобы [Required] заметил отсутствие значения в запросе.
// Ноль не подходит: калькулятор 1RM принимает только числа больше нуля.
public record UpdateWorkoutExerciseResultRequest(
    [Required, Range(0.01, 1000.0)] decimal? WorkingWeight,
    [Required, Range(1, 1000)] int? WorkingReps);
