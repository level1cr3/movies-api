using Microsoft.EntityFrameworkCore;
using Movies.Application.Models.Entities;

namespace Movies.Application.Data;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Movie> Movie { get; set; }
}