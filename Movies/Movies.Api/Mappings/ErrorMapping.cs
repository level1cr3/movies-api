using Movies.Application.Shared.Foundation;
using Movies.Contracts.Responses;

namespace Movies.Api.Mappings;

public static class ErrorMapping
{
    public static AppProblemDetails ToAppProblemDetails(this IReadOnlyList<AppError> appErrors, HttpContext httpContext)
    {
        return new AppProblemDetails
        {
            Title = "Request Failed",
            Detail = "One or more errors occured",
            Status = StatusCodes.Status400BadRequest,
            Type = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.1",
            Instance = httpContext.Request.Path,
            Method = httpContext.Request.Method,
            Errors = appErrors.Select(e => new ApiError(e.Code,e.Message,e.PropertyName)).ToList()
        };
    }
}