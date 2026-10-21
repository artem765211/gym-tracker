using Gymtracker.Core.Enums;

namespace Gymtracker.Core.Calculators;

public static class NutritionCalculator
{
    // Выбранные значения внутри диапазонов из плана курсовой
    private const decimal ProteinPerKg = 2.0m;   // диапазон 1,6–2,2 г на кг веса
    private const decimal FatPerKg = 0.9m;       // диапазон 0,8–1 г на кг веса

    private const decimal KcalPerGramProtein = 4m;
    private const decimal KcalPerGramCarbs = 4m;
    private const decimal KcalPerGramFat = 9m;

    /// <summary>Коэффициент активности (от 1,2 до 1,9).</summary>
    public static decimal ActivityFactor(ActivityLevel level) => level switch
    {
        ActivityLevel.Sedentary => 1.2m,
        ActivityLevel.Light => 1.375m,
        ActivityLevel.Moderate => 1.55m,
        ActivityLevel.High => 1.725m,
        ActivityLevel.VeryHigh => 1.9m,
        _ => throw new ArgumentOutOfRangeException(nameof(level))
    };

    /// <summary>Поправка калорий под цель: дефицит для похудения, профицит для набора.</summary>
    public static decimal GoalFactor(Goal goal) => goal switch
    {
        Goal.Lose => 0.85m,       // дефицит 15% (диапазон 10–20%)
        Goal.Maintain => 1.0m,
        Goal.Gain => 1.10m,       // профицит 10% (диапазон 10–15%)
        _ => throw new ArgumentOutOfRangeException(nameof(goal))
    };

    /// <summary>
    /// Суточная норма калорий и БЖУ.
    /// Калории = базовый обмен × коэффициент активности × поправка под цель.
    /// Белки и жиры считаются от веса, углеводы получают остаток калорий.
    /// </summary>
    public static NutritionResult Calculate(
        Sex sex,
        decimal weightKg,
        decimal heightCm,
        int age,
        ActivityLevel activityLevel,
        Goal goal)
    {
        // Проверка входных данных происходит внутри расчёта базового обмена
        var bmr = BmrCalculator.MifflinStJeor(sex, weightKg, heightCm, age);

        var calories = bmr * ActivityFactor(activityLevel) * GoalFactor(goal);

        var protein = weightKg * ProteinPerKg;
        var fat = weightKg * FatPerKg;

        var carbsKcal = calories - protein * KcalPerGramProtein - fat * KcalPerGramFat;
        var carbs = Math.Max(0m, carbsKcal / KcalPerGramCarbs);

        return new NutritionResult(
            Round(calories),
            Round(protein),
            Round(fat),
            Round(carbs));
    }

    private static decimal Round(decimal value) =>
        Math.Round(value, 0, MidpointRounding.AwayFromZero);
}
