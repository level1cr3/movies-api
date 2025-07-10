using Movies.Application.DTOs.Auth;
using Movies.Application.Shared.Foundation;

namespace Movies.Application.Features.Auth.Services;

public interface IAuthService
{
    Task RegisterAsync(RegisterDto register);
    
    Task<Result> ConfirmEmailAsync(string userId, string token);
    
}