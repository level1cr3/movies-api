using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Movies.Application.Data.Entities;
using Movies.Application.Data.Helpers;

namespace Movies.Application.Data.Configurations;

internal class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.ToTable("aspnet_roles").ConfigureGuidV7PrimaryKey(r => r.Id);
    }
}
