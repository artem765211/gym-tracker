using System.ComponentModel.DataAnnotations;

namespace Gymtracker.Api.Dtos.Workouts;

// Тип с ? нужен, чтобы [Required] заметил отсутствие значения в запросе.
public record UpdateWorkingWeightRequest(
    [Required, Range(0.01, 1000.0)] decimal? WorkingWeight);
