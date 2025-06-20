namespace Movies.Contracts.Responses.MovieResponse;

public record MovieResponse
{
    public required Guid Id { get; init; }

    public required string Title { get; init; }

    public required int YearOfRelease { get; set; }
    
    public required string Slug { get; init; }
}