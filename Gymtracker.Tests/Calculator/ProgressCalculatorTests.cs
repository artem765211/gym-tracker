using Gymtracker.Core.Calculators;
using Gymtracker.Core.Entities;
using Xunit;

namespace Gymtracker.Tests.Calculators;

public class ProgressCalculatorTests
{
    [Fact]
    public void PercentGrowth_From100To110_Returns10()
    {
        var result = ProgressCalculator.PercentGrowth(100m, 110m);

        Assert.Equal(10m, result);
    }

    [Fact]
    public void PercentGrowth_From110To100_ReturnsNegative()
    {
        var result = ProgressCalculator.PercentGrowth(110m, 100m);

        Assert.Equal(-9.09m, result, 2);
    }

    [Fact]
    public void PercentGrowth_SameValue_ReturnsZero()
    {
        var result = ProgressCalculator.PercentGrowth(100m, 100m);

        Assert.Equal(0m, result);
    }

    [Theory]
    [InlineData(0, 100)]
    [InlineData(-5, 100)]
    [InlineData(100, 0)]
    [InlineData(100, -5)]
    public void PercentGrowth_InvalidInput_Throws(int previous, int current)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => ProgressCalculator.PercentGrowth(previous, current));
    }

    private static StrengthRecord Record(string weekStart, decimal weight, int reps) => new()
    {
        WeekStart = DateOnly.Parse(weekStart),
        Weight = weight,
        Reps = reps
    };

    [Fact]
    public void ByMonth_NoRecords_ReturnsEmptyList()
    {
        var result = ProgressCalculator.ByMonth([]);

        Assert.Empty(result);
    }

    [Fact]
    public void ByMonth_FirstMonth_HasNoComparison()
    {
        var month = Assert.Single(ProgressCalculator.ByMonth([Record("2026-01-05", 100m, 5)]));

        Assert.Equal(new DateOnly(2026, 1, 1), month.Month);
        Assert.Equal(116.67m, month.BestOneRepMax);
        Assert.Null(month.PreviousMonth);
        Assert.Null(month.ChangeKg);
        Assert.Null(month.ChangePercent);
    }

    [Fact]
    public void ByMonth_SeveralRecordsInMonth_TakesBestOneRepMax()
    {
        // Вес меньше, но 80 × 10 (106,67) сильнее, чем 90 × 3 (99)
        var month = Assert.Single(ProgressCalculator.ByMonth(
        [
            Record("2026-01-05", 90m, 3),
            Record("2026-01-12", 80m, 10)
        ]));

        Assert.Equal(106.67m, month.BestOneRepMax);
    }

    [Fact]
    public void ByMonth_MonthWithoutRecords_ComparesWithNearestEarlierMonth()
    {
        var result = ProgressCalculator.ByMonth(
        [
            Record("2026-03-02", 110m, 5),
            Record("2026-01-05", 100m, 5)
        ]);

        Assert.Equal(2, result.Count);

        var march = result[1];
        Assert.Equal(new DateOnly(2026, 3, 1), march.Month);
        Assert.Equal(new DateOnly(2026, 1, 1), march.PreviousMonth);
        Assert.Equal(128.33m, march.BestOneRepMax);
        Assert.Equal(11.66m, march.ChangeKg);
        Assert.Equal(9.99m, march.ChangePercent);
    }

    [Fact]
    public void ByMonth_WeekCrossesMonth_BelongsToMonthOfMonday()
    {
        // Неделя с 26 октября по 1 ноября относится к октябрю
        var month = Assert.Single(ProgressCalculator.ByMonth([Record("2026-10-26", 100m, 5)]));

        Assert.Equal(new DateOnly(2026, 10, 1), month.Month);
    }

    [Fact]
    public void ByMonth_StrengthDropped_ReturnsNegativeChange()
    {
        var result = ProgressCalculator.ByMonth(
        [
            Record("2026-01-05", 100m, 5),
            Record("2026-02-02", 90m, 5)
        ]);

        Assert.Equal(-11.67m, result[1].ChangeKg);
        Assert.Equal(-10m, result[1].ChangePercent);
    }

    [Fact]
    public void ByMonth_AcrossYears_ComparesWithDecember()
    {
        var result = ProgressCalculator.ByMonth(
        [
            Record("2025-12-01", 100m, 5),
            Record("2026-01-05", 110m, 5)
        ]);

        Assert.Equal(new DateOnly(2025, 12, 1), result[1].PreviousMonth);
    }
}
