namespace Movies.Application.Features.Movie.DTOs;

public class MovieDto
{
    public required Guid Id { get; init; }

    public required string Title { get; init; }

    public required int YearOfRelease { get; init; }
    
    public required string Slug { get; init; }
}