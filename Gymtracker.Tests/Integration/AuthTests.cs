using System.Net;
using System.Net.Http.Json;
using Gymtracker.Api.Dtos.Auth;

namespace Gymtracker.Tests.Integration;

[Collection("Api")]
public class AuthTests
{
    private const string Password = "Test-password-1";

    private readonly HttpClient _client;

    public AuthTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    // База общая для всех тестов, поэтому у каждого теста свой email
    private static string NewEmail() => $"{Guid.NewGuid()}@example.test";

    [Fact]
    public async Task Register_ValidData_Returns201WithToken()
    {
        var email = NewEmail();

        var response = await _client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest(email, Password));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(body);
        Assert.Equal(email, body.Email);
        Assert.False(string.IsNullOrEmpty(body.Token));
    }

    [Fact]
    public async Task Login_CorrectPassword_Returns200WithToken()
    {
        var email = NewEmail();
        await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest(email, Password));

        var response = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest(email, Password));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(body);
        Assert.False(string.IsNullOrEmpty(body.Token));
    }

    [Fact]
    public async Task Login_WrongPassword_Returns401()
    {
        // Сначала регистрируем: иначе 401 мог бы означать, что такого пользователя просто нет
        var email = NewEmail();
        await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest(email, Password));

        var response = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest(email, "Wrong-password-1"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
