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

        if (exerciseIds.Distinct().Count() != exerciseIds.Count)
        {
            return BadRequestProblem("Одно упражнение нельзя добавить в тренировку дважды.");
        }

        // Добавлять можно только общие упражнения и свои личные
        var availableCount = await _db.Exercises
            .CountAsync(e => exerciseIds.Contains(e.Id) && (e.UserId == null || e.UserId == userId));

        if (availableCount != exerciseIds.Count)
        {
            return BadRequestProblem("Некоторые упражнения не найдены.");
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
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation
        })
        {
            // Два одинаковых запроса пришли почти одновременно (например, двойное нажатие):
            // оба прошли проверку выше, но второй остановил уникальный индекс в базе
            return BadRequestProblem(DuplicateDateMessage);
        }

        var response = await FindWorkoutResponse(workout.Id, userId);

        return CreatedAtAction(nameof(GetById), new { id = workout.Id }, response);
    }

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
