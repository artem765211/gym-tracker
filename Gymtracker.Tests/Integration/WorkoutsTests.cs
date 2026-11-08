using System.Net;
using System.Net.Http.Json;
using Gymtracker.Api.Dtos.Exercises;
using Gymtracker.Api.Dtos.Workouts;

namespace Gymtracker.Tests.Integration;

[Collection("Api")]
public class WorkoutsTests
{
    private readonly ApiFactory _factory;

    public WorkoutsTests(ApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetById_OtherUsersWorkout_Returns404()
    {
        // Подготовка: два разных пользователя, у первого есть тренировка
        var owner = await _factory.CreateAuthorizedClientAsync();
        var stranger = await _factory.CreateAuthorizedClientAsync();

        var exercises = await owner.GetFromJsonAsync<List<ExerciseResponse>>("/api/exercises");

        var created = await owner.PostAsJsonAsync(
            "/api/workouts",
            new CreateWorkoutRequest(new DateOnly(2026, 3, 10), null, [exercises![0].Id]));

        var workout = await created.Content.ReadFromJsonAsync<WorkoutResponse>();
        var url = $"/api/workouts/{workout!.Id}";

        // Действие: оба запрашивают одну и ту же тренировку
        var ownerResponse = await owner.GetAsync(url);
        var strangerResponse = await stranger.GetAsync(url);

        // Проверка: владелец её видит, а для другого пользователя её как будто нет
        Assert.Equal(HttpStatusCode.OK, ownerResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, strangerResponse.StatusCode);
    }

    [Fact]
    public async Task Create_DuplicateDate_Returns400AndKeepsOneWorkout()
    {
        // Подготовка: пользователь и запрос на создание тренировки
        var client = await _factory.CreateAuthorizedClientAsync();
        var exercises = await client.GetFromJsonAsync<List<ExerciseResponse>>("/api/exercises");

        var date = new DateOnly(2026, 3, 10);
        var request = new CreateWorkoutRequest(date, null, [exercises![0].Id]);

        // Действие: один и тот же запрос дважды
        var first = await client.PostAsJsonAsync("/api/workouts", request);
        var second = await client.PostAsJsonAsync("/api/workouts", request);

        // Проверка: первая тренировка создалась, вторая нет
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, second.StatusCode);

        // И в базе действительно осталась одна
        var workouts = await client.GetFromJsonAsync<List<WorkoutResponse>>(
            "/api/workouts?from=2026-03-10&to=2026-03-10");

        Assert.Single(workouts!);
    }
}
