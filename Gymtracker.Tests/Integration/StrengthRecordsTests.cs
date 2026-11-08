using System.Net;
using System.Net.Http.Json;
using Gymtracker.Api.Dtos.Exercises;
using Gymtracker.Api.Dtos.StrengthRecords;

namespace Gymtracker.Tests.Integration;

[Collection("Api")]
public class StrengthRecordsTests
{
    private readonly ApiFactory _factory;

    public StrengthRecordsTests(ApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Save_SameWeekTwice_ReplacesRecord()
    {
        // Подготовка: пользователь и упражнение
        var client = await _factory.CreateAuthorizedClientAsync();
        var exercises = await client.GetFromJsonAsync<List<ExerciseResponse>>("/api/exercises");
        var exerciseId = exercises![0].Id;

        // Действие: две записи за одну неделю, в понедельник 2 марта и в четверг 5 марта 2026.
        // Второй результат хуже первого: так видно, что сохраняется последний, а не лучший.
        var first = await client.PutAsJsonAsync(
            "/api/strength-records",
            new SaveStrengthRecordRequest(exerciseId, new DateOnly(2026, 3, 2), 85m, 6));

        var second = await client.PutAsJsonAsync(
            "/api/strength-records",
            new SaveStrengthRecordRequest(exerciseId, new DateOnly(2026, 3, 5), 80m, 5));

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.OK, second.StatusCode);

        // Проверка: запись одна, неделя начинается с понедельника, значения из последнего запроса
        var records = await client.GetFromJsonAsync<List<StrengthRecordResponse>>(
            $"/api/strength-records?exerciseId={exerciseId}");

        var record = Assert.Single(records!);
        Assert.Equal(new DateOnly(2026, 3, 2), record.WeekStart);
        Assert.Equal(80m, record.Weight);
        Assert.Equal(5, record.Reps);
    }
}
