using FluentValidation;
using Movies.Application.Data.Repositories;
using Movies.Application.Data.Repositories.Movies;
using Movies.Application.DTOs.Movies;
using Movies.Application.Mappings;

namespace Movies.Application.Services.Movies;

internal sealed class MovieService(
    IMovieRepository movieRepository,
    IUnitOfWork unitOfWork,
    IValidator<CreateMovieDto> createMovieDtoValidator) : IMovieService
{
    public async Task<MovieDto?> GetByIdAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        return await movieRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Guid?> CreateAsync(CreateMovieDto dto, CancellationToken cancellationToken = default)
    {
        await createMovieDtoValidator.ValidateAndThrowAsync(dto, cancellationToken);
        
        var movie = dto.ToMovie();
        movieRepository.Create(movie);
        var result = await unitOfWork.SaveChangesAsync(cancellationToken);
        return result > 0 ? movie.Id : null;
    }
    
}