using System.ComponentModel.DataAnnotations;

namespace Movies.Application.Data.Entities;

internal class Movie
{
    public required Guid Id { get; init; }

    [MaxLength(200)] 
    public required string Title { get; init; }

    public required int YearOfRelease { get; init; }

    [MaxLength(250)] 
    public required string Slug { get; init; }
}