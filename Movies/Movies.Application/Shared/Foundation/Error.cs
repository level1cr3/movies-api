namespace Movies.Application.Shared.Foundation;

public record Error(string Code, string Message,string? PropertyName = null)
{

}