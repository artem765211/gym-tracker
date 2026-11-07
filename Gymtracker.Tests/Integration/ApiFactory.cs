using Gymtracker.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Gymtracker.Tests.Integration;

// Запускает API прямо внутри тестов и подключает его к временной базе в Docker.
// WebApplicationFactory<Program> поднимает приложение из Program.cs без настоящего порта.
// IAsyncLifetime добавляет шаги «перед всеми тестами» и «после всех тестов».
public class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    // Ключ только для тестов, настоящий лежит в user-secrets
    private const string TestJwtKey = "test-only-signing-key-at-least-32-bytes-long";

    // Описание контейнера: такая же версия PostgreSQL, как в docker-compose.yml
    private readonly PostgreSqlContainer _database = new PostgreSqlBuilder("postgres:16").Build();

    public string ConnectionString => _database.GetConnectionString();

    // Перед всеми тестами: запускаем контейнер и создаём в нём таблицы по нашим миграциям
    public async Task InitializeAsync()
    {
        await _database.StartAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        await using var db = new AppDbContext(options);
        await db.Database.MigrateAsync();
    }

    // Подменяем настройки приложения: другая база и другой ключ подписи токенов
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:DefaultConnection", ConnectionString);
        builder.UseSetting("Jwt:Key", TestJwtKey);
    }

    // После всех тестов: останавливаем API и удаляем контейнер вместе с данными
    async Task IAsyncLifetime.DisposeAsync()
    {
        await base.DisposeAsync();
        await _database.DisposeAsync();
    }
}
