using System.ComponentModel.DataAnnotations;

namespace Gymtracker.Api.Dtos.Auth;

public record LoginRequest(
    [Required, EmailAddress, MaxLength(256)] string Email,
    [Required, MaxLength(100)] string Password);
