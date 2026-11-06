using System.ComponentModel.DataAnnotations;

namespace Gymtracker.Api.Dtos.Workouts;

// DateOnly? нужен, чтобы [Required] заметил отсутствие даты в запросе.
// С обычным DateOnly вместо неё молча подставилось бы 01.01.0001.
public record UpdateWorkoutRequest(
    [Required] DateOnly? Date,
    [MaxLength(500)] string? Note,
    [Required, MinLength(1)] List<int> ExerciseIds);
