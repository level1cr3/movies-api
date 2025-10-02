using Movies.Application.Shared.DTOs;
using Movies.Application.Shared.Foundation;

namespace Movies.Application.Features.UserManagement.Services;

public interface IUserService
{
    Task<Result<UserDto>> GetCurrentUserInfoAsync(CancellationToken cancellationToken = default);
}