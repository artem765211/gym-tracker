using System.Security.Claims;
using Gymtracker.Api.Dtos.Nutrition;
using Gymtracker.Core.Calculators;
using Gymtracker.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gymtracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/nutrition")]
public class NutritionController : ControllerBase
{
    private readonly AppDbContext _db;

    public NutritionController(AppDbContext db)
    {
        _db = db;
    }

    // Id пользователя берём из токена, а не из запроса
    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // Норма КБЖУ по самому позднему замеру пользователя
    [HttpGet("calculate")]
    public async Task<ActionResult<NutritionResponse>> Calculate()
    {
        var userId = CurrentUserId;

        var measurement = await _db.BodyMeasurements
            .AsNoTracking()
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.Date)
            .FirstOrDefaultAsync();

        if (measurement is null)
        {
            return Problem(
                detail: "Сначала добавьте замер тела.",
                statusCode: StatusCodes.Status404NotFound);
        }

        // Возраст считаем на сегодня, а не на дату замера: так норма верна и по старому замеру
        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = AgeCalculator.FromBirthDate(measurement.BirthDate, today);

        var result = NutritionCalculator.Calculate(
            measurement.Sex,
            measurement.Weight,
            measurement.Height,
            age,
            measurement.ActivityLevel,
            measurement.Goal);

        // При крайних значениях (очень маленькие вес и рост, большой возраст) формула даёт ноль или минус
        if (result.Calories <= 0)
        {
            return Problem(
                detail: "По этому замеру норму рассчитать нельзя. Проверьте вес, рост и дату рождения.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        return Ok(new NutritionResponse(
            measurement.Date,
            result.Calories,
            result.ProteinGrams,
            result.FatGrams,
            result.CarbsGrams));
    }
}
