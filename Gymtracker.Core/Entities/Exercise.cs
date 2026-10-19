namespace Gymtracker.Core.Entities;

public class Exercise
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string MuscleGroup { get; set; } = string.Empty;

    // null = общее упражнение для всех, иначе Id пользователя-владельца
    public string? UserId { get; set; }
}
