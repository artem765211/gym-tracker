namespace Gymtracker.Core.Calculators;

public static class WeekCalculator
{
    /// <summary>
    /// Понедельник той недели, в которую попадает дата.
    /// </summary>
    public static DateOnly GetWeekStart(DateOnly date)
    {
        // В DayOfWeek воскресенье имеет номер 0, поэтому сдвигаем так, чтобы понедельник стал 0, а воскресенье 6
        var daysSinceMonday = ((int)date.DayOfWeek + 6) % 7;

        return date.AddDays(-daysSinceMonday);
    }
}
