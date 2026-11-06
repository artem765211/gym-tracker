namespace Gymtracker.Core.Interfaces;

public interface ITokenService
{
    string CreateToken(string userId, string email);
}
