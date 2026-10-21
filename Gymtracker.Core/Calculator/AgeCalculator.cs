namespace Gymtracker.Core.Calculators;

public static class AgeCalculator
{
    /// <summary>
    /// Полных лет на указанную дату. Дату «сегодня» передаём параметром,
    /// чтобы расчёт можно было проверить тестами.
    /// </summary>
    public static int FromBirthDate(DateOnly birthDate, DateOnly today)
    {
        if (birthDate > today)
            throw new ArgumentOutOfRangeException(nameof(birthDate), "Дата рождения не может быть в будущем.");

        var age = today.Year - birthDate.Year;

        // День рождения в этом году ещё не наступил — год не набрался
        if (today < birthDate.AddYears(age))
            age--;

        return age;
    }
}
