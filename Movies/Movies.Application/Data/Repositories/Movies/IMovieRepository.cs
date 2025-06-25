using Movies.Application.Data.Entities;
using Movies.Application.DTOs.Movies;

namespace Movies.Application.Data.Repositories.Movies;

internal interface IMovieRepository
{
    Task<MovieDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<MovieDto?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<IEnumerable<MovieDto>> GetAllAsync(CancellationToken cancellationToken = default);
    
    
    void Create(Movie movie);

    void Update(Movie movie);

    void Delete(Guid id, CancellationToken cancellationToken = default);
}