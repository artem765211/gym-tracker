using Gymtracker.Core.Calculators;
using Xunit;

namespace Gymtracker.Tests.Calculators;

public class AgeCalculatorTests
{
    private static readonly DateOnly Birth = new(2000, 6, 15);

    [Fact]
    public void FromBirthDate_BirthdayToday_CountsFullYear()
    {
        var age = AgeCalculator.FromBirthDate(Birth, new DateOnly(2026, 6, 15));

        Assert.Equal(26, age);
    }

    [Fact]
    public void FromBirthDate_DayBeforeBirthday_NotYetFullYear()
    {
        var age = AgeCalculator.FromBirthDate(Birth, new DateOnly(2026, 6, 14));

        Assert.Equal(25, age);
    }

    [Fact]
    public void FromBirthDate_AfterBirthdayInYear_CountsFullYear()
    {
        var age = AgeCalculator.FromBirthDate(Birth, new DateOnly(2026, 12, 31));

        Assert.Equal(26, age);
    }

    [Fact]
    public void FromBirthDate_BirthDateInFuture_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => AgeCalculator.FromBirthDate(new DateOnly(2030, 1, 1), new DateOnly(2026, 6, 15)));
    }
}
