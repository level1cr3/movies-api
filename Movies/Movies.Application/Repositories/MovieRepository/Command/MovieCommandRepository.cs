using Movies.Application.Data;
using Movies.Application.Models.Entities;

namespace Movies.Application.Repositories.MovieRepository.Command;

internal sealed class MovieCommandRepository(ApplicationDbContext db) : IMovieCommandRepository
{
    public void Create(Movie movie)
    {
        db.Movies.Add(movie);
    }

    public void Update(Movie movie)
    {
        db.Movies.Update(movie);
    }

    public void Delete(Movie movie)
    {
        db.Movies.Remove(movie);
    }
}