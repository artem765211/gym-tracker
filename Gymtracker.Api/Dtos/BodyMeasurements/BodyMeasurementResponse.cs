using Gymtracker.Core.Enums;

namespace Gymtracker.Api.Dtos.BodyMeasurements;

public record BodyMeasurementResponse(
    int Id,
    DateOnly Date,
    decimal Weight,
    decimal Height,
    DateOnly BirthDate,
    Sex Sex,
    ActivityLevel ActivityLevel,
    Goal Goal);
