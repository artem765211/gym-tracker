using Gymtracker.Core.Calculators;
using Xunit;

namespace Gymtracker.Tests.Calculators;

public class WeekCalculatorTests
{
    [Theory]
    [InlineData("2026-11-02")] // понедельник
    [InlineData("2026-11-04")] // среда
    [InlineData("2026-11-08")] // воскресенье
    public void GetWeekStart_AnyDayOfWeek_ReturnsMonday(string date)
    {
        var result = WeekCalculator.GetWeekStart(DateOnly.Parse(date));

        Assert.Equal(new DateOnly(2026, 11, 2), result);
    }

    [Fact]
    public void GetWeekStart_WeekCrossesMonth_ReturnsMondayOfPreviousMonth()
    {
        var result = WeekCalculator.GetWeekStart(new DateOnly(2026, 11, 1)); // воскресенье

        Assert.Equal(new DateOnly(2026, 10, 26), result);
    }

    [Fact]
    public void GetWeekStart_WeekCrossesYear_ReturnsMondayOfPreviousYear()
    {
        var result = WeekCalculator.GetWeekStart(new DateOnly(2027, 1, 1)); // пятница

        Assert.Equal(new DateOnly(2026, 12, 28), result);
    }
}
