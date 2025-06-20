namespace Movies.Contracts.Requests.MovieRequest;

public record GetMovieRequest
{
    public required Guid Id { get; init; }
}