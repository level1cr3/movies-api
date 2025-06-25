namespace Movies.Application.DTOs.Movies;

public record CreateMovieDto
{
    public string Title { get; init; } = string.Empty;

    public int YearOfRelease { get; init; }
}