using FluentValidation;
using Movies.Application.Models.Queries.MovieQueries;

namespace Movies.Application.Validators.MovieValidators;

public sealed class GetMovieByIdQueryValidator : AbstractValidator<GetMovieByIdQuery>
{
    public GetMovieByIdQueryValidator()
    {
        RuleFor(q => q.Id).NotEmpty();
    }
}