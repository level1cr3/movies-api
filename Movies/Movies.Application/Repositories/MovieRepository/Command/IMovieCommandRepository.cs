using Movies.Application.Models.Entities;

namespace Movies.Application.Repositories.MovieRepository.Command;

internal interface IMovieCommandRepository
{
    void Create(Movie movie);

    void Update(Movie movie);

    void Delete(Movie movie);
}