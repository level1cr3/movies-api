using Movies.Application.Models.Entities;

namespace Movies.Application.Repositories.MovieRepository;

public interface IMovieRepository
{
    Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<Movie?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<IEnumerable<Movie>> GetAllAsync(CancellationToken cancellationToken = default);

    void Create(Movie movie, CancellationToken cancellationToken = default);

    void Update(Movie movie, CancellationToken cancellationToken = default);

    Task Delete(Guid id, CancellationToken cancellationToken = default);
    
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default);
}