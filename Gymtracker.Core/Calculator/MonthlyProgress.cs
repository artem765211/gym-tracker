namespace Gymtracker.Core.Calculators;

/// <summary>
/// Лучший 1RM за месяц и сравнение с ближайшим предыдущим месяцем, где были записи.
/// Месяцы заданы первым днём. У самого первого месяца поля сравнения пустые.
/// </summary>
public record MonthlyProgress(
    DateOnly Month,
    decimal BestOneRepMax,
    DateOnly? PreviousMonth,
    decimal? ChangeKg,
    decimal? ChangePercent);
