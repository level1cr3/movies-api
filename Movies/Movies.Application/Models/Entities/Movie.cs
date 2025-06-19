using System.ComponentModel.DataAnnotations;

namespace Movies.Application.Models.Entities;

public class Movie
{
    public required Guid Id { get; set; }

    [MaxLength(200)] public required string Title { get; set; }

    public required int YearOfRelease { get; set; }

    [MaxLength(250)] public required string Slug { get; set; }
}