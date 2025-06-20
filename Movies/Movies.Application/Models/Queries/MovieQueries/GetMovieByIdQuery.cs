namespace Movies.Application.Models.Queries.MovieQueries;

public record GetMovieByIdQuery
{
    public Guid Id { get; init; } 
}