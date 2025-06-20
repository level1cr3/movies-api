namespace Movies.Application.Models.Aggregates.MovieAggregates;

public class MovieAggregate
{
    public required Guid Id { get; set; }

    public required string Title { get; set; }

    public required int YearOfRelease { get; set; }
    
    public required string Slug { get; set; }
}