using Gymtracker.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gymtracker.Infrastructure.Data;

public static class ExerciseSeeder
{
    /// <summary>
    /// Заполняет справочник общими упражнениями (UserId = null).
    /// Если общие упражнения в базе уже есть, ничего не делает.
    /// </summary>
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Exercises.AnyAsync(e => e.UserId == null))
            return;

        db.Exercises.AddRange(
            // Грудь
            Common("Жим лёжа", "Грудь"),
            Common("Жим гантелей лёжа", "Грудь"),
            Common("Жим лёжа на наклонной скамье", "Грудь"),
            Common("Разводка гантелей лёжа", "Грудь"),
            Common("Сведение рук в тренажёре", "Грудь"),

            // Спина
            Common("Становая тяга", "Спина"),
            Common("Тяга штанги в наклоне", "Спина"),
            Common("Тяга верхнего блока", "Спина"),
            Common("Тяга нижнего блока", "Спина"),
            Common("Тяга гантели одной рукой", "Спина"),

            // Ноги
            Common("Приседания со штангой", "Ноги"),
            Common("Жим ногами", "Ноги"),
            Common("Румынская тяга", "Ноги"),
            Common("Выпады с гантелями", "Ноги"),
            Common("Разгибание ног в тренажёре", "Ноги"),
            Common("Сгибание ног в тренажёре", "Ноги"),
            Common("Подъём на носки стоя", "Ноги"),

            // Плечи
            Common("Жим штанги стоя", "Плечи"),
            Common("Жим гантелей сидя", "Плечи"),
            Common("Махи гантелей в стороны", "Плечи"),

            // Руки
            Common("Подъём штанги на бицепс", "Бицепс"),
            Common("Молотковые сгибания", "Бицепс"),
            Common("Французский жим", "Трицепс"),
            Common("Разгибание рук на верхнем блоке", "Трицепс"),

            // Пресс
            Common("Скручивания на блоке", "Пресс"));

        await db.SaveChangesAsync();
    }

    private static Exercise Common(string name, string muscleGroup) =>
        new() { Name = name, MuscleGroup = muscleGroup, UserId = null };
}
