using Microsoft.EntityFrameworkCore;
using Movies.Application.Data.Entities;
using Movies.Application.DTOs.Movies;
using Movies.Application.Mappings;

namespace Movies.Application.Data.Repositories.Movies;

internal class MovieRepository(ApplicationDbContext db) : IMovieRepository
{
    public async Task<MovieDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await db.Movies.Where(m => m.Id == id)
            .Select(m => m.ToMovieDto())
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<MovieDto?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await db.Movies.Where(m => m.Slug == slug)
            .Select(m => m.ToMovieDto())
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<MovieDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var movies = await db.Movies.ToListAsync(cancellationToken);
        return movies.ToMovieDtoList();
    }

    public void Create(Movie movie)
    {
        db.Movies.Add(movie);
    }

    public void Update(Movie movie)
    {
        throw new NotImplementedException();
    }

    public void Delete(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}