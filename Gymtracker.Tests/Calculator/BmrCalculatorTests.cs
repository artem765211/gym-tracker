using Gymtracker.Core.Calculators;
using Gymtracker.Core.Enums;
using Xunit;

namespace Gymtracker.Tests.Calculators;

public class BmrCalculatorTests
{
    [Fact]
    public void MifflinStJeor_Male_80kg_180cm_25y_Returns1805()
    {
        // 10*80 + 6.25*180 - 5*25 + 5 = 800 + 1125 - 125 + 5
        var result = BmrCalculator.MifflinStJeor(Sex.Male, 80m, 180m, 25);

        Assert.Equal(1805m, result);
    }

    [Fact]
    public void MifflinStJeor_Female_60kg_165cm_30y_Returns1320_25()
    {
        // 10*60 + 6.25*165 - 5*30 - 161 = 600 + 1031.25 - 150 - 161
        var result = BmrCalculator.MifflinStJeor(Sex.Female, 60m, 165m, 30);

        Assert.Equal(1320.25m, result);
    }

    [Theory]
    [InlineData(0, 180, 25)]
    [InlineData(-1, 180, 25)]
    [InlineData(80, 0, 25)]
    [InlineData(80, 180, 0)]
    public void MifflinStJeor_InvalidInput_Throws(int weight, int height, int age)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => BmrCalculator.MifflinStJeor(Sex.Male, weight, height, age));
    }
}
