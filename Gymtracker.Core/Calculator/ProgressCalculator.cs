namespace Gymtracker.Core.Calculators;

public static class ProgressCalculator
{
    /// <summary>
    /// Прирост за месяц в процентах:
    /// (лучший 1RM этого месяца − лучший 1RM прошлого месяца) / лучший 1RM прошлого месяца × 100.
    /// Результат отрицательный, если сила упала.
    /// </summary>
    public static decimal PercentGrowth(decimal previousBest, decimal currentBest)
    {
        if (previousBest <= 0)
            throw new ArgumentOutOfRangeException(nameof(previousBest), "Прошлый результат должен быть больше нуля.");

        if (currentBest <= 0)
            throw new ArgumentOutOfRangeException(nameof(currentBest), "Текущий результат должен быть больше нуля.");

        return (currentBest - previousBest) / previousBest * 100;
    }
}
