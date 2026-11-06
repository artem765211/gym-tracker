using System.ComponentModel.DataAnnotations;

namespace Gymtracker.Api.Dtos.Exercises;

public record CreateExerciseRequest(
    [Required, MaxLength(100)] string Name,
    [Required, MaxLength(50)] string MuscleGroup);
