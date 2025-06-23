using Microsoft.EntityFrameworkCore;
using Movies.Application.Models.Entities;

namespace Movies.Application.Data;

internal class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Movie> Movies { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IApplicationAssemblyMarker).Assembly);
    }
}