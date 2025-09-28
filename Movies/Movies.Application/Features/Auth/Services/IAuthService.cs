using Movies.Application.Features.Auth.DTOs;
using Movies.Application.Shared.Foundation;

namespace Movies.Application.Features.Auth.Services;

public interface IAuthService
{
    Task<Result> RegisterAsync(RegisterDto register, CancellationToken cancellationToken = default);
    
    Task<Result> ConfirmEmailAsync(string userId, string token, CancellationToken cancellationToken = default);
    
    Task<Result> ResendConfirmEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<Result<LoginDto>> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    
    Task<Result<AuthTokenDto>> RefreshTokenAsync(string token, CancellationToken cancellationToken = default);

    Task<Result> LogoutAsync(string token, CancellationToken cancellationToken = default);

    Task<Result> ForgotPasswordAsync(string email, CancellationToken cancellationToken = default);

    Task<Result> ResetPasswordAsync(string userId, string token, string newPassword, CancellationToken cancellationToken = default);
    
    
}