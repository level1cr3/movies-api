namespace Movies.Application.Models.Aggregates.MovieAggregates;

public class MovieAggregateList
{
    public required IEnumerable<MovieAggregate> Movies { get; init; } = [];
}