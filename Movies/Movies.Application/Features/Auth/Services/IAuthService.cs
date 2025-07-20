using Movies.Application.Features.Auth.DTOs;
using Movies.Application.Shared.Foundation;

namespace Movies.Application.Features.Auth.Services;

public interface IAuthService
{
    Task<Result> RegisterAsync(RegisterDto register);
    
    Task<Result> ConfirmEmailAsync(string userId, string token);

    Task<Result<AuthTokenDto>> LoginAsync(string email, string password);
    
    Task<Result<AuthTokenDto>> RefreshTokenAsync(string token);
}