using Movies.Application.Shared.Foundation;

namespace Movies.Application.Features.Auth.Errors;

public static class EmailConfirmationErrors
{
    public static readonly AppError InvalidRequest 
        = new("EmailConfirmation.InvalidRequest", "Unable to confirm email.");
    
    
}