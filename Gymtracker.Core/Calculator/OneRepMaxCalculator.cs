namespace Gymtracker.Core.Calculators;

public static class OneRepMaxCalculator
{
    /// <summary>
    /// Расчётный разовый максимум (1RM) по формуле Эпли:
    /// 1RM = вес × (1 + повторения / 30).
    /// </summary>
    public static decimal Epley(decimal weight, int reps)
    {
        if (weight <= 0)
            throw new ArgumentOutOfRangeException(nameof(weight), "Вес должен быть больше нуля.");

        if (reps <= 0)
            throw new ArgumentOutOfRangeException(nameof(reps), "Число повторений должно быть больше нуля.");

        return weight * (1 + reps / 30m);
    }
}
