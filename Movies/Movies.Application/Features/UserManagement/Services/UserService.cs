using Microsoft.AspNetCore.Identity;
using Movies.Application.Data.Entities;
using Movies.Application.Features.UserManagement.Errors;
using Movies.Application.Shared.DTOs;
using Movies.Application.Shared.Foundation;
using Movies.Application.Shared.Services;

namespace Movies.Application.Features.UserManagement.Services;

internal class UserService(
    UserManager<ApplicationUser> userManager,
    ICurrentUserService currentUserService) : IUserService
{
    public async Task<Result<UserDto>> GetCurrentUserInfoAsync(CancellationToken cancellationToken = default)
    {
        if (currentUserService.UserId is null)
        {
            return Result.Failure<UserDto>([UserErrors.UserNotFound]);
        }
        
        var user = await userManager.FindByIdAsync(currentUserService.UserId);

        if (user is null)
        {
            return Result.Failure<UserDto>([UserErrors.UserNotFound]);
        }

        var roles = await userManager.GetRolesAsync(user);

        var fullname = $"{user.FirstName} {user.LastName}";
        var userDto = new UserDto(user.Id,
            fullname,
            user.Email!,
            roles);
        return Result.Success(userDto);
    }
}