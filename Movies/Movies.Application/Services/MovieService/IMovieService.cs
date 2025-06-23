using Movies.Application.Models.Aggregates.MovieAggregates;
using Movies.Application.Models.Commands.MovieCommands;
using Movies.Application.Models.Queries.MovieQueries;

namespace Movies.Application.Services.MovieService;

public interface IMovieService
{
    Task<MovieAggregate?> GetByIdAsync(GetMovieByIdQuery query, CancellationToken cancellationToken = default);

    Task<Guid?> CreateAsync(CreateMovieCommand command, CancellationToken cancellationToken = default);
}