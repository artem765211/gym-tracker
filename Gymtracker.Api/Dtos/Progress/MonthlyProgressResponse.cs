namespace Gymtracker.Api.Dtos.Progress;

// Месяцы передаём строкой вида "2026-03".
// У самого первого месяца с записями поля сравнения равны null.
public record MonthlyProgressResponse(
    string Month,
    decimal BestOneRepMax,
    string? PreviousMonth,
    decimal? ChangeKg,
    decimal? ChangePercent);
