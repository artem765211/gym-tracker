using Gymtracker.Api.Dtos.Auth;
using Gymtracker.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Gymtracker.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ITokenService _tokenService;

    public AuthController(UserManager<IdentityUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var user = new IdentityUser { UserName = request.Email, Email = request.Email };

        // UserManager сам хеширует пароль и проверяет его сложность и уникальность email
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }

            return ValidationProblem(ModelState);
        }

        var token = _tokenService.CreateToken(user.Id, request.Email);

        return StatusCode(StatusCodes.Status201Created, new AuthResponse(token, request.Email));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        // Одинаковый ответ в обоих случаях, чтобы нельзя было узнать, какие email зарегистрированы
        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Invalid email or password.");
        }

        var token = _tokenService.CreateToken(user.Id, user.Email!);

        return Ok(new AuthResponse(token, user.Email!));
    }
}
