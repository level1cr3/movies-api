namespace Movies.Application.Shared.Foundation;

public record AppError(string Code, string Message,string? PropertyName = null);