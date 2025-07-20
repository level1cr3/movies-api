using Movies.Application.Shared.Foundation;

namespace Movies.Application.Features.Auth.Errors;

public static class RefreshTokenErrors
{
    public static readonly AppError Invalid = new("RefreshToken.Invalid","Invalid token.");
}