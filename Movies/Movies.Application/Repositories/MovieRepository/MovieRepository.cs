using Microsoft.EntityFrameworkCore;
using Movies.Application.Data;
using Movies.Application.Models.Entities;

namespace Movies.Application.Repositories.MovieRepository;

public class MovieRepository(ApplicationDbContext db) : IMovieRepository
{
    public async Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await db.Movie.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<Movie?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await db.Movie.FirstOrDefaultAsync(m => m.Slug == slug, cancellationToken);
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await db.Movie.ToListAsync(cancellationToken);
    }

    void IMovieRepository.Create(Movie movie, CancellationToken cancellationToken)
    {
        db.Movie.Add(movie);
    }

    public void Update(Movie movie, CancellationToken cancellationToken = default)
    {
        db.Movie.Update(movie);
    }

    public async Task Delete(Guid id, CancellationToken cancellationToken = default)
    {
       var movie = await db.Movie.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
       
       if (movie is null)
       {
           return;
       }
       
       db.Movie.Remove(movie);
    } 

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await db.Movie.AnyAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await db.Movie.AnyAsync(m => m.Slug == slug, cancellationToken);
    }
}