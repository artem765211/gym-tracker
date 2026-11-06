using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Gymtracker.Api.Dtos.Workouts;
using Gymtracker.Core.Entities;
using Gymtracker.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Gymtracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/workouts")]
public class WorkoutsController : ControllerBase
{
    private const string DuplicateDateMessage = "На эту дату у вас уже есть тренировка.";

    private readonly AppDbContext _db;

    public WorkoutsController(AppDbContext db)
    {
        _db = db;
    }

    // Id пользователя берём из токена, а не из запроса
    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // Календарь запрашивает тренировки за нужный период: GET /api/workouts?from=2026-11-01&to=2026-11-30
    [HttpGet]
    public async Task<ActionResult<List<WorkoutResponse>>> GetByPeriod(
        [FromQuery, Required] DateOnly? from,
        [FromQuery, Required] DateOnly? to)
    {
        if (from > to)
        {
            return BadRequestProblem("Дата from не может быть позже даты to.");
        }

        var userId = CurrentUserId;

        var workouts = await ToResponse(_db.Workouts
                .AsNoTracking()
                .Where(w => w.UserId == userId && w.Date >= from && w.Date <= to)
                .OrderBy(w => w.Date))
            .ToListAsync();

        return Ok(workouts);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<WorkoutResponse>> GetById(int id)
    {
        var workout = await FindWorkoutResponse(id, CurrentUserId);

        // Чужая тренировка даёт 404, как и несуществующая
        return workout is null ? NotFound() : Ok(workout);
    }

    [HttpPost]
    public async Task<ActionResult<WorkoutResponse>> Create(CreateWorkoutRequest request)
    {
        var userId = CurrentUserId;
        var date = request.Date!.Value; // [Required] уже проверил, что дата есть
        var exerciseIds = request.ExerciseIds;

        var exercisesError = await CheckExercises(exerciseIds, userId);
        if (exercisesError is not null)
        {
            return BadRequestProblem(exercisesError);
        }

        if (await _db.Workouts.AnyAsync(w => w.UserId == userId && w.Date == date))
        {
            return BadRequestProblem(DuplicateDateMessage);
        }

        var workout = new Workout
        {
            UserId = userId,
            Date = date,
            Note = request.Note,
            Exercises = exerciseIds
                .Select(exerciseId => new WorkoutExercise { ExerciseId = exerciseId })
                .ToList()
        };

        _db.Workouts.Add(workout);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            // Два одинаковых запроса пришли почти одновременно (например, двойное нажатие):
            // оба прошли проверку выше, но второй остановил уникальный индекс в базе
            return BadRequestProblem(DuplicateDateMessage);
        }

        var response = await FindWorkoutResponse(workout.Id, userId);

        return CreatedAtAction(nameof(GetById), new { id = workout.Id }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<WorkoutResponse>> Update(int id, UpdateWorkoutRequest request)
    {
        var userId = CurrentUserId;
        var date = request.Date!.Value; // [Required] уже проверил, что дата есть
        var exerciseIds = request.ExerciseIds;

        var workout = await _db.Workouts
            .Include(w => w.Exercises)
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

        // Чужая тренировка даёт 404, как и несуществующая
        if (workout is null)
        {
            return NotFound();
        }

        var exercisesError = await CheckExercises(exerciseIds, userId);
        if (exercisesError is not null)
        {
            return BadRequestProblem(exercisesError);
        }

        // Эту же тренировку не считаем: дата может остаться прежней
        if (await _db.Workouts.AnyAsync(w => w.UserId == userId && w.Date == date && w.Id != id))
        {
            return BadRequestProblem(DuplicateDateMessage);
        }

        workout.Date = date;
        workout.Note = request.Note;

        // Оставшиеся упражнения не трогаем, чтобы сохранить их вес и повторения.
        // Убранные удаляются вместе с результатами, новые добавляются пустыми.
        workout.Exercises.RemoveAll(we => !exerciseIds.Contains(we.ExerciseId));

        var currentIds = workout.Exercises.Select(we => we.ExerciseId).ToList();
        workout.Exercises.AddRange(exerciseIds
            .Except(currentIds)
            .Select(exerciseId => new WorkoutExercise { ExerciseId = exerciseId }));

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex is DbUpdateConcurrencyException || IsUniqueViolation(ex))
        {
            // Почти одновременно пришёл другой запрос и изменил те же данные (например, двойное нажатие)
            return BadRequestProblem("Не удалось сохранить: данные только что изменились. Обновите их и попробуйте снова.");
        }

        return Ok(await FindWorkoutResponse(id, userId));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = CurrentUserId;

        // Упражнения тренировки удалит сама база (каскадное удаление в AppDbContext).
        // Метод возвращает количество удалённых строк: 0 значит, что тренировки нет или она чужая.
        var deletedCount = await _db.Workouts
            .Where(w => w.Id == id && w.UserId == userId)
            .ExecuteDeleteAsync();

        return deletedCount == 0 ? NotFound() : NoContent();
    }

    // Возвращает текст ошибки или null, если список упражнений подходит
    private async Task<string?> CheckExercises(List<int> exerciseIds, string userId)
    {
        if (exerciseIds.Distinct().Count() != exerciseIds.Count)
        {
            return "Одно упражнение нельзя добавить в тренировку дважды.";
        }

        // Добавлять можно только общие упражнения и свои личные
        var availableCount = await _db.Exercises
            .CountAsync(e => exerciseIds.Contains(e.Id) && (e.UserId == null || e.UserId == userId));

        return availableCount == exerciseIds.Count ? null : "Некоторые упражнения не найдены.";
    }

    private static bool IsUniqueViolation(DbUpdateException ex) =>
        ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };

    private ObjectResult BadRequestProblem(string detail) =>
        Problem(detail: detail, statusCode: StatusCodes.Status400BadRequest);

    private Task<WorkoutResponse?> FindWorkoutResponse(int id, string userId) =>
        ToResponse(_db.Workouts
                .AsNoTracking()
                .Where(w => w.Id == id && w.UserId == userId))
            .FirstOrDefaultAsync();

    // Общий формат ответа: какие данные тренировки достать из базы и как их вернуть
    private static IQueryable<WorkoutResponse> ToResponse(IQueryable<Workout> workouts) =>
        workouts.Select(w => new WorkoutResponse(
            w.Id,
            w.Date,
            w.Note,
            w.Exercises
                .OrderBy(we => we.Id)
                .Select(we => new WorkoutExerciseResponse(
                    we.ExerciseId,
                    we.Exercise.Name,
                    we.Exercise.MuscleGroup,
                    we.WorkingWeight,
                    we.WorkingReps))
                .ToList()));
}
