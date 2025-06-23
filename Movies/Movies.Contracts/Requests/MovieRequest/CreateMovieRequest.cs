namespace Movies.Contracts.Requests.MovieRequest;

public record CreateMovieRequest
{
    public required string Title { get; init; }
    
    public required int YearOfRelease { get; init; }
}