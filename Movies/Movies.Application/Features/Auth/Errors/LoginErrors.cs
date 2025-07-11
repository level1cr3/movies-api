using Movies.Application.Shared.Foundation;

namespace Movies.Application.Features.Auth.Errors;

public static class LoginErrors
{
    public static readonly AppError NotAllowed = new("Login.NotAllowed", 
        "Your account is not allowed to sign in. Please confirm your email");
    
    public static readonly AppError LockedOut = new("Login.LockedOut", "User account is locked out.");
    
    public static readonly AppError Invalid = new("Login.Invalid", "Invalid login attempt.");
}