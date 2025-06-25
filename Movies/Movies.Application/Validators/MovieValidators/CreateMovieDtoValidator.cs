using FluentValidation;
using Movies.Application.DTOs.Movies;

namespace Movies.Application.Validators.MovieValidators;

internal class CreateMovieDtoValidator : AbstractValidator<CreateMovieDto>
{
    public CreateMovieDtoValidator()
    {
        RuleFor(c => c.Title).NotEmpty().MaximumLength(200);

        RuleFor(c => c.YearOfRelease).InclusiveBetween(1900, DateTime.UtcNow.Year);
    }
}