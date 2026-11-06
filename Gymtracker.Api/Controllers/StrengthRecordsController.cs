using System.Security.Claims;
using Gymtracker.Api.Dtos.StrengthRecords;
using Gymtracker.Core.Calculators;
using Gymtracker.Core.Entities;
using Gymtracker.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Gymtracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/strength-records")]
public class StrengthRecordsController : ControllerBase
{
    private readonly AppDbContext _db;

    public StrengthRecordsController(AppDbContext db)
    {
        _db = db;
    }

    // Id пользователя берём из токена, а не из запроса
    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // История записей, сначала новые: GET /api/strength-records?exerciseId=3 (фильтр необязательный)
    [HttpGet]
    public async Task<ActionResult<List<StrengthRecordResponse>>> GetAll([FromQuery] int? exerciseId)
    {
        var userId = CurrentUserId;

        var records = _db.StrengthRecords
            .AsNoTracking()
            .Where(r => r.UserId == userId);

        if (exerciseId is not null)
        {
            records = records.Where(r => r.ExerciseId == exerciseId);
        }

        return Ok(await ToResponse(records
                .OrderByDescending(r => r.WeekStart)
                .ThenBy(r => r.ExerciseId))
            .ToListAsync());
    }

    // Один адрес и для новой записи, и для исправления: за неделю по упражнению хранится один результат
    [HttpPut]
    public async Task<ActionResult<StrengthRecordResponse>> Save(SaveStrengthRecordRequest request)
    {
        var userId = CurrentUserId;
        var exerciseId = request.ExerciseId!.Value; // [Required] уже проверил, что значения есть
        var weight = request.Weight!.Value;
        var reps = request.Reps!.Value;
        var weekStart = WeekCalculator.GetWeekStart(request.Date!.Value);

        // Сегодняшнюю дату берём с сервера. Будущие недели исказили бы прогресс.
        var currentWeekStart = WeekCalculator.GetWeekStart(DateOnly.FromDateTime(DateTime.Today));
        if (weekStart > currentWeekStart)
        {
            return BadRequestProblem("Нельзя записать результат за будущую неделю.");
        }

        // Записывать можно только общие упражнения и свои личные
        var exerciseAvailable = await _db.Exercises
            .AnyAsync(e => e.Id == exerciseId && (e.UserId == null || e.UserId == userId));
        if (!exerciseAvailable)
        {
            return BadRequestProblem("Упражнение не найдено.");
        }

        // Метод возвращает количество изменённых строк: 0 значит, что записи за эту неделю ещё нет
        if (await UpdateExisting(userId, exerciseId, weekStart, weight, reps) == 0)
        {
            _db.StrengthRecords.Add(new StrengthRecord
            {
                UserId = userId,
                ExerciseId = exerciseId,
                WeekStart = weekStart,
                Weight = weight,
                Reps = reps
            });

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                // Два запроса пришли почти одновременно (например, двойное нажатие),
                // и первый уже создал запись. Значит, этот запрос её просто обновит.
                await UpdateExisting(userId, exerciseId, weekStart, weight, reps);
            }
        }

        var response = await ToResponse(_db.StrengthRecords
                .AsNoTracking()
                .Where(r => r.UserId == userId && r.ExerciseId == exerciseId && r.WeekStart == weekStart))
            .FirstAsync();

        return Ok(response);
    }

    private Task<int> UpdateExisting(string userId, int exerciseId, DateOnly weekStart, decimal weight, int reps) =>
        _db.StrengthRecords
            .Where(r => r.UserId == userId && r.ExerciseId == exerciseId && r.WeekStart == weekStart)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(r => r.Weight, weight)
                .SetProperty(r => r.Reps, reps));

    private static bool IsUniqueViolation(DbUpdateException ex) =>
        ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };

    private ObjectResult BadRequestProblem(string detail) =>
        Problem(detail: detail, statusCode: StatusCodes.Status400BadRequest);

    private static IQueryable<StrengthRecordResponse> ToResponse(IQueryable<StrengthRecord> records) =>
        records.Select(r => new StrengthRecordResponse(
            r.Id,
            r.ExerciseId,
            r.Exercise.Name,
            r.Exercise.MuscleGroup,
            r.WeekStart,
            r.Weight,
            r.Reps));
}
