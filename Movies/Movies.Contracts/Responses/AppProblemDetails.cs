namespace Movies.Contracts.Responses;

public record AppProblemDetails
{
    public required string Title { get; init; }
    
    public required string Detail { get; init; }
    
    public required int Status { get; init; }
    
    public required string Type { get; init; }
    
    public required string Instance { get; init; }
    
    public required string Method { get; init; }

    public required IReadOnlyList<ApiError> Errors  { get; init; } 
}

public record ApiError(string Code, string Message,string? PropertyName = null);