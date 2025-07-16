using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Movies.Application.Data.Entities;

namespace Movies.Application.Data;

internal class ApplicationDbContext(DbContextOptions options)
    : IdentityDbContext<ApplicationUser,ApplicationRole,Guid>(options)
{
    public DbSet<Movie> Movies { get; set; }

    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IApplicationAssemblyMarker).Assembly);
        
        // rename tables according to postgresql
        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("aspnet_user_roles");
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("aspnet_user_claims");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("aspnet_user_logins");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("aspnet_role_claims");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("aspnet_user_tokens");
    }
}