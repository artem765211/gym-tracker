using Gymtracker.Core.Calculators;
using Xunit;

namespace Gymtracker.Tests.Calculators;

public class OneRepMaxCalculatorTests
{
    [Fact]
    public void Epley_100kgFor3Reps_Returns110()
    {
        var result = OneRepMaxCalculator.Epley(100m, 3);

        Assert.Equal(110m, result);
    }

    [Fact]
    public void Epley_100kgFor10Reps_ReturnsAbout133()
    {
        var result = OneRepMaxCalculator.Epley(100m, 10);

        Assert.Equal(133.33m, result, 2);
    }

    [Fact]
    public void Epley_SameWeightMoreReps_GivesHigherResult()
    {
        var fewReps = OneRepMaxCalculator.Epley(100m, 3);
        var manyReps = OneRepMaxCalculator.Epley(100m, 10);

        Assert.True(manyReps > fewReps);
    }

    [Theory]
    [InlineData(0, 5)]
    [InlineData(-10, 5)]
    [InlineData(100, 0)]
    [InlineData(100, -1)]
    public void Epley_InvalidInput_Throws(int weight, int reps)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => OneRepMaxCalculator.Epley(weight, reps));
    }
}
