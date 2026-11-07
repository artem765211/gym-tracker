using System.ComponentModel.DataAnnotations;
using Gymtracker.Core.Enums;

namespace Gymtracker.Api.Dtos.BodyMeasurements;

// Типы с ? нужны, чтобы [Required] заметил отсутствие значения в запросе.
// [EnumDataType] не пропускает значения вне перечисления: например, "High, VeryHigh" дало бы число без названия.
public record CreateBodyMeasurementRequest(
    [Required] DateOnly? Date,
    [Required, Range(20.0, 500.0)] decimal? Weight,
    [Required, Range(50.0, 300.0)] decimal? Height,
    [Required] DateOnly? BirthDate,
    [Required, EnumDataType(typeof(Sex))] Sex? Sex,
    [Required, EnumDataType(typeof(ActivityLevel))] ActivityLevel? ActivityLevel,
    [Required, EnumDataType(typeof(Goal))] Goal? Goal);
