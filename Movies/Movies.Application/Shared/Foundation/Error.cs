namespace Movies.Application.Shared.Foundation;

public record Error(string Code, string Message, string? PropertyName = null)
{
    public static readonly Error None = new(string.Empty, string.Empty);
}