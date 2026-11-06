namespace Gymtracker.Infrastructure.Auth;

// Заполняется из секции "Jwt" конфигурации. Key хранится в user-secrets, остальное в appsettings.json
public class JwtOptions
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public int ExpiresMinutes { get; set; }
}
