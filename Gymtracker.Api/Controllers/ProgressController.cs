using System.Globalization;
using System.Security.Claims;
using Gymtracker.Api.Dtos.Progress;
using Gymtracker.Core.Calculators;
using Gymtracker.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gymtracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/progress")]
public class ProgressController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProgressController(AppDbContext db)
    {
        _db = db;
    }

    // Id пользователя берём из токена, а не из запроса
    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet("exercises/{id:int}")]
    public async Task<ActionResult<ExerciseProgressResponse>> GetByExercise(int id)
    {
        var userId = CurrentUserId;

        var exercise = await _db.Exercises
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id && (e.UserId == null || e.UserId == userId));

        // Чужое упражнение даёт 404, как и несуществующее
        if (exercise is null)
        {
            return NotFound();
        }

        var records = await _db.StrengthRecords
            .AsNoTracking()
            .Where(r => r.UserId == userId && r.ExerciseId == id)
            .ToListAsync();

        var months = ProgressCalculator.ByMonth(records)
            .Select(ToResponse)
            .ToList();

        return Ok(new ExerciseProgressResponse(exercise.Id, exercise.Name, exercise.MuscleGroup, months));
    }

    // Только упражнения, по которым есть записи
    [HttpGet("summary")]
    public async Task<ActionResult<List<ExerciseProgressSummaryResponse>>> GetSummary()
    {
        var userId = CurrentUserId;

        var records = await _db.StrengthRecords
            .AsNoTracking()
            .Include(r => r.Exercise)
            .Where(r => r.UserId == userId)
            .ToListAsync();

        var summary = records
            .GroupBy(r => r.ExerciseId)
            .Select(group =>
            {
                var exercise = group.First().Exercise;

                return new ExerciseProgressSummaryResponse(
                    exercise.Id,
                    exercise.Name,
                    exercise.MuscleGroup,
                    ToResponse(ProgressCalculator.ByMonth(group).Last()));
            })
            .OrderBy(s => s.ExerciseName)
            .ToList();

        return Ok(summary);
    }

    private static MonthlyProgressResponse ToResponse(MonthlyProgress month) =>
        new(
            FormatMonth(month.Month),
            month.BestOneRepMax,
            month.PreviousMonth is { } previous ? FormatMonth(previous) : null,
            month.ChangeKg,
            month.ChangePercent);

    private static string FormatMonth(DateOnly month) =>
        month.ToString("yyyy-MM", CultureInfo.InvariantCulture);
}
