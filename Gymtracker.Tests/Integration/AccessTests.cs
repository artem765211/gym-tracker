using System.Net;
using Gymtracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Gymtracker.Tests.Integration;

[Collection("Api")]
public class AccessTests
{
    private readonly ApiFactory _factory;
    private readonly HttpClient _client;

    // xUnit сам передаёт сюда общий ApiFactory
    public AccessTests(ApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient(); // клиент, который шлёт запросы прямо в запущенный в тесте API
    }

    // Защита: тесты не должны случайно работать с настоящей базой
    [Fact]
    public void Api_UsesTemporaryTestDatabase()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Assert.Equal(_factory.ConnectionString, db.Database.GetConnectionString());
    }

    [Fact]
    public async Task GetWorkouts_WithoutToken_Returns401()
    {
        // Отправляем запрос без токена
        var response = await _client.GetAsync("/api/workouts?from=2026-01-01&to=2026-01-31");

        // Проверяем код ответа
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
