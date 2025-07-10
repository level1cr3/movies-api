using Microsoft.AspNetCore.Identity;
using Movies.Application.Shared.Foundation;

namespace Movies.Application.Features.Auth.Mappings;

public static class AuthMappings
{
    public static List<AppError> ToAppErrors(this IEnumerable<IdentityError> identityErrors)
    {
        return identityErrors.Select(e => new AppError(e.Code, e.Description)).ToList();
    }
}