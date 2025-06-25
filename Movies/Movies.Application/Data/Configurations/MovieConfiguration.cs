using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Movies.Application.Data.Entities;

namespace Movies.Application.Data.Configurations;

internal sealed class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.HasIndex(movie => movie.Slug)
            .IsUnique().HasDatabaseName("movie_slug_idx");
    }
}