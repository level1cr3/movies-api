using Movies.Application.Shared.Foundation;

namespace Movies.Application.Features.UserManagement.Errors;

public static class UserErrors
{
    public static readonly AppError UserNotFound = new ("User.NotFound","User not found");
}