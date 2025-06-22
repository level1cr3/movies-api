using Movies.Application.Models.Aggregates.MovieAggregates;

namespace Movies.Application.Repositories.MovieRepository.Query;

internal interface IMovieQueryRepository
{
    Task<MovieAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<MovieAggregate?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<MovieAggregateList> GetAllAsync(CancellationToken cancellationToken = default);
    
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default);
}