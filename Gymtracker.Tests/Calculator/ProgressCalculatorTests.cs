using Gymtracker.Core.Calculators;
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
}
