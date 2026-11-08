using System.Net.Http.Json;
using Gymtracker.Api.Dtos.Exercises;
using Gymtracker.Api.Dtos.Progress;
using Gymtracker.Api.Dtos.StrengthRecords;

namespace Gymtracker.Tests.Integration;

[Collection("Api")]
public class ProgressTests
{
    private readonly ApiFactory _factory;

    public ProgressTests(ApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetByExercise_MonthWithoutRecords_ComparesWithNearestEarlierMonth()
    {
        // Подготовка: записи в январе и марте, в феврале ничего нет
        var client = await _factory.CreateAuthorizedClientAsync();
        var exercises = await client.GetFromJsonAsync<List<ExerciseResponse>>("/api/exercises");
        var exerciseId = exercises![0].Id;

        // 1RM = вес × (1 + повторения / 30): 100 × 5 даёт 116,67, а 110 × 5 даёт 128,33
        await client.PutAsJsonAsync("/api/strength-records",
            new SaveStrengthRecordRequest(exerciseId, new DateOnly(2026, 1, 5), 100m, 5));
        await client.PutAsJsonAsync("/api/strength-records",
            new SaveStrengthRecordRequest(exerciseId, new DateOnly(2026, 3, 2), 110m, 5));

        // Действие
        var progress = await client.GetFromJsonAsync<ExerciseProgressResponse>(
            $"/api/progress/exercises/{exerciseId}");

        // Проверка: два месяца, март сравнивается с январём
        Assert.Equal(2, progress!.Months.Count);

        var january = progress.Months[0];
        Assert.Equal("2026-01", january.Month);
        Assert.Null(january.PreviousMonth); // первому месяцу не с чем сравнивать

        var march = progress.Months[1];
        Assert.Equal("2026-03", march.Month);
        Assert.Equal("2026-01", march.PreviousMonth);
        Assert.Equal(128.33m, march.BestOneRepMax);
        Assert.Equal(11.66m, march.ChangeKg);   // 128,33 − 116,67
        Assert.Equal(9.99m, march.ChangePercent); // 11,66 / 116,67 × 100
    }
}
