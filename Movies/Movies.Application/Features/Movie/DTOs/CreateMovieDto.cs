namespace Movies.Application.Features.Movie.DTOs;

public record CreateMovieDto
{
    public string Title { get; init; } = string.Empty;

    public int YearOfRelease { get; init; }
}