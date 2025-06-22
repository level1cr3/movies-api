namespace Movies.Application.Models.Aggregates.MovieAggregates;

public class MovieAggregate
{
    public required Guid Id { get; init; }

    public required string Title { get; init; }

    public required int YearOfRelease { get; init; }
    
    public required string Slug { get; init; }
}