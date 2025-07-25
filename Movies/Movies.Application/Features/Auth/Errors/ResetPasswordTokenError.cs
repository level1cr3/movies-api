using Movies.Application.Shared.Foundation;
using Org.BouncyCastle.Utilities.IO;

namespace Movies.Application.Features.Auth.Errors;

public static class ResetPasswordTokenError
{
    public static readonly AppError Invalid = new("ResetPasswordToken.Invalid", "Invalid token");
}