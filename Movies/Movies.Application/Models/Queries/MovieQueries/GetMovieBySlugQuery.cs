namespace Movies.Application.Models.Queries.MovieQueries;

public record GetMovieBySlugQuery
{
    public string Slug { get; init; } = string.Empty;
}