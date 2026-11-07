namespace Gymtracker.Api.Dtos.Nutrition;

// MeasurementDate показывает, по какому замеру сделан расчёт.
public record NutritionResponse(
    DateOnly MeasurementDate,
    decimal Calories,
    decimal ProteinGrams,
    decimal FatGrams,
    decimal CarbsGrams);
