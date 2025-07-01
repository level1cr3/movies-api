using Movies.Application.DTOs.Auth;

namespace Movies.Application.Services.Auth;

public interface IAuthService
{
    Task RegisterAsync(RegisterDto register);
}