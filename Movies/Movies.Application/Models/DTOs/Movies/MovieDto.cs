namespace Movies.Application.Models.DTOs.Movies;

public class MovieDto
{
    public required Guid Id { get; init; }

    public required string Title { get; init; }

    public required int YearOfRelease { get; init; }
    
    public required string Slug { get; init; }
}