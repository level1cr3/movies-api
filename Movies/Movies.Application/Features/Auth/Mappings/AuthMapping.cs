using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Movies.Application.Shared.Foundation;

namespace Movies.Application.Features.Auth.Mappings;

public static class AuthMapping
{
    public static List<AppError> ToAppErrors(this IEnumerable<IdentityError> identityErrors)
    {
        return identityErrors.Select(e => new AppError(e.Code, e.Description)).ToList();
    }

    public static List<AppError> ToAppErrors(this IEnumerable<ValidationFailure> validationFailures)
    {
        return validationFailures.Select(e => new AppError(e.ErrorCode, e.ErrorMessage,e.PropertyName)).ToList();
    }
    
    
}