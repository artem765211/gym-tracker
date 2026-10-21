using Gymtracker.Core.Calculators;
using Gymtracker.Core.Enums;
using Xunit;

namespace Gymtracker.Tests.Calculators;

public class NutritionCalculatorTests
{
    [Fact]
    public void ActivityFactor_Sedentary_Returns1_2()
    {
        Assert.Equal(1.2m, NutritionCalculator.ActivityFactor(ActivityLevel.Sedentary));
    }

    [Fact]
    public void ActivityFactor_VeryHigh_Returns1_9()
    {
        Assert.Equal(1.9m, NutritionCalculator.ActivityFactor(ActivityLevel.VeryHigh));
    }

    [Fact]
    public void Calculate_Maintain_Male80kg180cm25y_Moderate()
    {
        // Базовый обмен 1805, × 1,55 = 2797,75
        // Белки 80 × 2,0 = 160 г, жиры 80 × 0,9 = 72 г
        // Углеводы: (2797,75 − 160×4 − 72×9) / 4 ≈ 377 г
        var result = NutritionCalculator.Calculate(
            Sex.Male, 80m, 180m, 25, ActivityLevel.Moderate, Goal.Maintain);

        Assert.Equal(2798m, result.Calories);
        Assert.Equal(160m, result.ProteinGrams);
        Assert.Equal(72m, result.FatGrams);
        Assert.Equal(377m, result.CarbsGrams);
    }

    [Fact]
    public void Calculate_Lose_AppliesDeficit()
    {
        // 2797,75 × 0,85 = 2378,09; углеводы ≈ 273 г
        var result = NutritionCalculator.Calculate(
            Sex.Male, 80m, 180m, 25, ActivityLevel.Moderate, Goal.Lose);

        Assert.Equal(2378m, result.Calories);
        Assert.Equal(273m, result.CarbsGrams);
    }

    [Fact]
    public void Calculate_Gain_AppliesSurplus()
    {
        // 2797,75 × 1,10 = 3077,53; углеводы ≈ 447 г
        var result = NutritionCalculator.Calculate(
            Sex.Male, 80m, 180m, 25, ActivityLevel.Moderate, Goal.Gain);

        Assert.Equal(3078m, result.Calories);
        Assert.Equal(447m, result.CarbsGrams);
    }

    [Fact]
    public void Calculate_NotEnoughCaloriesForProteinAndFat_CarbsNeverNegative()
    {
        // Тяжёлый человек с малой активностью и дефицитом: белков и жиров
        // по весу получается больше калорий, чем в норме, углеводы обнуляются
        var result = NutritionCalculator.Calculate(
            Sex.Female, 150m, 150m, 60, ActivityLevel.Sedentary, Goal.Lose);

        Assert.Equal(0m, result.CarbsGrams);
    }

    [Fact]
    public void Calculate_InvalidWeight_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => NutritionCalculator.Calculate(
                Sex.Male, 0m, 180m, 25, ActivityLevel.Moderate, Goal.Maintain));
    }
}
