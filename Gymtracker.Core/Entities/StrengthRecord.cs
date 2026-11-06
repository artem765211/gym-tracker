namespace Gymtracker.Core.Entities;

// Рабочий вес и повторения за неделю (вкладка «Сила»). С тренировками не связано.
public class StrengthRecord
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;

    public int ExerciseId { get; set; }
    public Exercise Exercise { get; set; } = null!;

    public DateOnly WeekStart { get; set; }   // понедельник этой недели

    public decimal Weight { get; set; }   // кг
    public int Reps { get; set; }
}
