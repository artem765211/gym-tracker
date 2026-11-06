using System.ComponentModel.DataAnnotations;

namespace Gymtracker.Api.Dtos.StrengthRecords;

// Типы с ? нужны, чтобы [Required] заметил отсутствие значения в запросе.
// Date может быть любым днём недели: сервер сам найдёт её понедельник.
// Ноль не подходит: калькулятор 1RM принимает только числа больше нуля.
public record SaveStrengthRecordRequest(
    [Required] int? ExerciseId,
    [Required] DateOnly? Date,
    [Required, Range(0.01, 1000.0)] decimal? Weight,
    [Required, Range(1, 1000)] int? Reps);
