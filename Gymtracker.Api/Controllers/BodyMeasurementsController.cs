using System.Security.Claims;
using Gymtracker.Api.Dtos.BodyMeasurements;
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
[Route("api/body-measurements")]
public class BodyMeasurementsController : ControllerBase
{
    private const string DuplicateDateMessage = "На эту дату у вас уже есть замер.";
    private const int MinAge = 10;
    private const int MaxAge = 120;

    private readonly AppDbContext _db;

    public BodyMeasurementsController(AppDbContext db)
    {
        _db = db;
    }

    // Id пользователя берём из токена, а не из запроса
    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // История замеров, сначала новые
    [HttpGet]
    public async Task<ActionResult<List<BodyMeasurementResponse>>> GetAll()
    {
        var userId = CurrentUserId;

        var measurements = await _db.BodyMeasurements
            .AsNoTracking()
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.Date)
            .Select(m => ToResponse(m))
            .ToListAsync();

        return Ok(measurements);
    }

    [HttpPost]
    public async Task<ActionResult<BodyMeasurementResponse>> Create(CreateBodyMeasurementRequest request)
    {
        var userId = CurrentUserId;
        var date = request.Date!.Value; // [Required] уже проверил, что значения есть
        var birthDate = request.BirthDate!.Value;
        var today = DateOnly.FromDateTime(DateTime.Today); // сегодняшнюю дату берём с сервера

        if (date > today)
        {
            return BadRequestProblem("Дата замера не может быть в будущем.");
        }

        if (birthDate >= date)
        {
            return BadRequestProblem("Дата рождения должна быть раньше даты замера.");
        }

        // Расчёт КБЖУ работает только с возрастом больше нуля
        var age = AgeCalculator.FromBirthDate(birthDate, today);
        if (age < MinAge || age > MaxAge)
        {
            return BadRequestProblem($"Возраст должен быть от {MinAge} до {MaxAge} лет.");
        }

        if (await _db.BodyMeasurements.AnyAsync(m => m.UserId == userId && m.Date == date))
        {
            return BadRequestProblem(DuplicateDateMessage);
        }

        var measurement = new BodyMeasurement
        {
            UserId = userId,
            Date = date,
            Weight = request.Weight!.Value,
            Height = request.Height!.Value,
            BirthDate = birthDate,
            Sex = request.Sex!.Value,
            ActivityLevel = request.ActivityLevel!.Value,
            Goal = request.Goal!.Value
        };

        _db.BodyMeasurements.Add(measurement);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            // Два одинаковых запроса пришли почти одновременно (например, двойное нажатие):
            // оба прошли проверку выше, но второй остановил уникальный индекс в базе
            return BadRequestProblem(DuplicateDateMessage);
        }

        return StatusCode(StatusCodes.Status201Created, ToResponse(measurement));
    }

    private ObjectResult BadRequestProblem(string detail) =>
        Problem(detail: detail, statusCode: StatusCodes.Status400BadRequest);

    private static BodyMeasurementResponse ToResponse(BodyMeasurement m) =>
        new(m.Id, m.Date, m.Weight, m.Height, m.BirthDate, m.Sex, m.ActivityLevel, m.Goal);
}
