using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Movies.Application.Models.Entities;

namespace Movies.Application.Data.Configurations;

public sealed class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.HasIndex(movie => movie.Slug)
            .IsUnique().HasDatabaseName("movie_slug_idx");
    }
}