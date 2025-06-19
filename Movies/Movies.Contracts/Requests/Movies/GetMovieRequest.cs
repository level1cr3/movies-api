namespace Movies.Contracts.Requests.Movies;

public record GetMovieRequest
{
    public required Guid Id { get; init; }
}