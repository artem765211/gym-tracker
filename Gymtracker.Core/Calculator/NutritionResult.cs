namespace Gymtracker.Core.Calculators;

/// <summary>Результат расчёта КБЖУ: калории в сутки и граммы белков, жиров и углеводов.</summary>
public record NutritionResult(
    decimal Calories,
    decimal ProteinGrams,
    decimal FatGrams,
    decimal CarbsGrams);
