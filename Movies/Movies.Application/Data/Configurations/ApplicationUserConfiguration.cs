using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Movies.Application.Data.Entities;
using Movies.Application.Data.Helpers;

namespace Movies.Application.Data.Configurations;

internal class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("aspnet_users").ConfigureGuidV7PrimaryKey(u => u.Id);
    }
}