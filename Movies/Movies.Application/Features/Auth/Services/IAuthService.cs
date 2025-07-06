using Movies.Application.DTOs.Auth;

namespace Movies.Application.Features.Auth.Services;

public interface IAuthService
{
    Task RegisterAsync(RegisterDto register);
    
    Task ConfirmEmailAsync(string userId, string token);
    
}