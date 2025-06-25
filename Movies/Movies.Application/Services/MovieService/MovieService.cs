using FluentValidation;
using Movies.Application.Mappings;
using Movies.Application.Models.Commands.MovieCommands;
using Movies.Application.Models.DTOs.Movies;
using Movies.Application.Repositories;
using Movies.Application.Repositories.Movies;

namespace Movies.Application.Services.MovieService;

internal sealed class MovieService(
    IMovieRepository movieRepository,
    IUnitOfWork unitOfWork,
    IValidator<CreateMovieDto> createMovieValidator) : IMovieService
{
    public async Task<MovieDto?> GetByIdAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        return await movieRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Guid?> CreateAsync(CreateMovieDto dto, CancellationToken cancellationToken = default)
    {
        await createMovieValidator.ValidateAndThrowAsync(dto, cancellationToken);
        
        var movie = dto.ToMovie();
        movieRepository.Create(movie);
        var result = await unitOfWork.SaveChangesAsync(cancellationToken);
        return result > 0 ? movie.Id : null;
    }
    
}