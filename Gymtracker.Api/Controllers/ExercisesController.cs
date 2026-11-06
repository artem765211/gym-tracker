using System.Security.Claims;
using Gymtracker.Api.Dtos.Exercises;
using Gymtracker.Core.Entities;
using Gymtracker.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gymtracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/exercises")]
public class ExercisesController : ControllerBase
{
    private readonly AppDbContext _db;

    public ExercisesController(AppDbContext db)
    {
        _db = db;
    }

    // Id пользователя берём из токена, а не из запроса
    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<ActionResult<List<ExerciseResponse>>> GetAll()
    {
        var userId = CurrentUserId;

        // Пользователю видны общие упражнения и только его личные
        var exercises = await _db.Exercises
            .AsNoTracking()
            .Where(e => e.UserId == null || e.UserId == userId)
            .OrderBy(e => e.Name)
            .Select(e => new ExerciseResponse(e.Id, e.Name, e.MuscleGroup, e.UserId != null))
            .ToListAsync();

        return Ok(exercises);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ExerciseResponse>> GetById(int id)
    {
        var userId = CurrentUserId;

        var exercise = await _db.Exercises
            .AsNoTracking()
            .Where(e => e.Id == id && (e.UserId == null || e.UserId == userId))
            .Select(e => new ExerciseResponse(e.Id, e.Name, e.MuscleGroup, e.UserId != null))
            .FirstOrDefaultAsync();

        // Чужое упражнение даёт 404, как и несуществующее
        return exercise is null ? NotFound() : Ok(exercise);
    }

    [HttpPost]
    public async Task<ActionResult<ExerciseResponse>> Create(CreateExerciseRequest request)
    {
        var exercise = new Exercise
        {
            Name = request.Name,
            MuscleGroup = request.MuscleGroup,
            UserId = CurrentUserId
        };

        _db.Exercises.Add(exercise);
        await _db.SaveChangesAsync();

        var response = new ExerciseResponse(exercise.Id, exercise.Name, exercise.MuscleGroup, true);

        return CreatedAtAction(nameof(GetById), new { id = exercise.Id }, response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = CurrentUserId;

        var exercise = await _db.Exercises
            .FirstOrDefaultAsync(e => e.Id == id && (e.UserId == null || e.UserId == userId));

        // Чужое упражнение даёт 404, как и несуществующее
        if (exercise is null)
        {
            return NotFound();
        }

        // Общее упражнение пользователь видит, но удалять его нельзя
        if (exercise.UserId is null)
        {
            return Problem(
                detail: "Общие упражнения из справочника удалять нельзя.",
                statusCode: StatusCodes.Status403Forbidden);
        }

        // Проверяем заранее: база всё равно не даст удалить такое упражнение,
        // но пользователь получил бы непонятную ошибку 500
        var isUsed = await _db.WorkoutExercises.AnyAsync(we => we.ExerciseId == id)
            || await _db.StrengthRecords.AnyAsync(r => r.ExerciseId == id);
        if (isUsed)
        {
            return Problem(
                detail: "Упражнение используется в тренировках или рабочих весах, поэтому удалить его нельзя.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        _db.Exercises.Remove(exercise);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
