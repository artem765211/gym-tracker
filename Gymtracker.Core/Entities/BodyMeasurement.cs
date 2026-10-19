using Gymtracker.Core.Enums;

namespace Gymtracker.Core.Entities;

public class BodyMeasurement
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateOnly Date { get; set; }

    public decimal Weight { get; set; }   // кг
    public decimal Height { get; set; }   // см
    public DateOnly BirthDate { get; set; }

    public Sex Sex { get; set; }
    public ActivityLevel ActivityLevel { get; set; }
    public Goal Goal { get; set; }
}
