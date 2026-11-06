using Gymtracker.Core.Entities;

namespace Gymtracker.Core.Calculators;

public static class ProgressCalculator
{
    /// <summary>
    /// Прирост в процентах:
    /// (лучший 1RM этого месяца − лучший 1RM предыдущего месяца с записями) / лучший 1RM предыдущего месяца с записями × 100.
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

    /// <summary>
    /// Прогресс по месяцам для записей одного упражнения, от старых месяцев к новым.
    /// Неделя относится к месяцу своего понедельника.
    /// Месяцы без записей пропускаются: сравнение идёт с ближайшим месяцем, где записи есть.
    /// </summary>
    public static List<MonthlyProgress> ByMonth(IEnumerable<StrengthRecord> records)
    {
        var result = new List<MonthlyProgress>();

        var months = records
            .GroupBy(r => new DateOnly(r.WeekStart.Year, r.WeekStart.Month, 1))
            .OrderBy(group => group.Key);

        foreach (var month in months)
        {
            var best = Round(month.Max(r => OneRepMaxCalculator.Epley(r.Weight, r.Reps)));
            var previous = result.LastOrDefault();

            // Сравниваем округлённые значения, чтобы разница сходилась с числами, которые видит пользователь
            result.Add(new MonthlyProgress(
                month.Key,
                best,
                previous?.Month,
                previous is null ? null : best - previous.BestOneRepMax,
                previous is null ? null : Round(PercentGrowth(previous.BestOneRepMax, best))));
        }

        return result;
    }

    private static decimal Round(decimal value) =>
        Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
