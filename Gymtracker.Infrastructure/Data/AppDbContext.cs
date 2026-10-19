using Gymtracker.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gymtracker.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Каждый DbSet = одна таблица в базе
    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<Workout> Workouts => Set<Workout>();
    public DbSet<WorkoutExercise> WorkoutExercises => Set<WorkoutExercise>();
    public DbSet<BodyMeasurement> BodyMeasurements => Set<BodyMeasurement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Exercise>(e =>
        {
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.Property(x => x.MuscleGroup).IsRequired().HasMaxLength(50);
            e.Property(x => x.UserId).HasMaxLength(450);
            e.HasIndex(x => x.UserId);
        });

        modelBuilder.Entity<Workout>(w =>
        {
            w.Property(x => x.UserId).IsRequired().HasMaxLength(450);
            w.Property(x => x.Note).HasMaxLength(500);
            w.HasIndex(x => new { x.UserId, x.Date });

            // Удалили тренировку: её упражнения удаляются вместе с ней
            w.HasMany(x => x.Exercises)
                .WithOne(x => x.Workout)
                .HasForeignKey(x => x.WorkoutId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<WorkoutExercise>(we =>
        {
            // Упражнение, которое уже используется в тренировках, удалить нельзя
            we.HasOne(x => x.Exercise)
                .WithMany()
                .HasForeignKey(x => x.ExerciseId)
                .OnDelete(DeleteBehavior.Restrict);

            we.Property(x => x.TopWeight).HasPrecision(6, 2);
        });

        modelBuilder.Entity<BodyMeasurement>(m =>
        {
            m.Property(x => x.UserId).IsRequired().HasMaxLength(450);
            m.Property(x => x.Weight).HasPrecision(6, 2);
            m.Property(x => x.Height).HasPrecision(5, 1);

            // Перечисления хранятся в базе текстом (Male, Lose, Light...)
            m.Property(x => x.Sex).HasConversion<string>().HasMaxLength(20);
            m.Property(x => x.ActivityLevel).HasConversion<string>().HasMaxLength(20);
            m.Property(x => x.Goal).HasConversion<string>().HasMaxLength(20);

            m.HasIndex(x => new { x.UserId, x.Date });
        });
    }
}
