using Gymtracker.Core.Enums;

namespace Gymtracker.Core.Calculators;

public static class BmrCalculator
{
    /// <summary>
    /// Базовый обмен веществ (ккал в сутки) по формуле Миффлина — Сан-Жеора:
    /// мужчины: 10 × вес + 6,25 × рост − 5 × возраст + 5
    /// женщины: 10 × вес + 6,25 × рост − 5 × возраст − 161
    /// </summary>
    public static decimal MifflinStJeor(Sex sex, decimal weightKg, decimal heightCm, int age)
    {
        if (weightKg <= 0)
            throw new ArgumentOutOfRangeException(nameof(weightKg), "Вес должен быть больше нуля.");

        if (heightCm <= 0)
            throw new ArgumentOutOfRangeException(nameof(heightCm), "Рост должен быть больше нуля.");

        if (age <= 0)
            throw new ArgumentOutOfRangeException(nameof(age), "Возраст должен быть больше нуля.");

        var sexConstant = sex switch
        {
            Sex.Male => 5m,
            Sex.Female => -161m,
            _ => throw new ArgumentOutOfRangeException(nameof(sex))
        };

        return 10m * weightKg + 6.25m * heightCm - 5m * age + sexConstant;
    }
}
