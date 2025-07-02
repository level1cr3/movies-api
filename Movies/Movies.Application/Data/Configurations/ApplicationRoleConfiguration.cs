using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Movies.Application.Constants;
using Movies.Application.Data.Entities;
using Movies.Application.Data.Helpers;

namespace Movies.Application.Data.Configurations;

internal class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.ToTable("aspnet_roles").ConfigureGuidV7PrimaryKey(r => r.Id);

        builder.HasData(
            new ApplicationRole
            {
                Id = new Guid("0197c5c8-8105-7c77-805a-4fe55026f743"), // manually inserting keep it same across. because we don't want to change it in case it was bound with user.
                Name = Role.Admin,
                NormalizedName = Role.Admin.ToUpper(),
                ConcurrencyStamp = "34b57f91-badf-4c9f-ae96-5122a04efa9a"
            },
            new ApplicationRole
            {
                Id = new Guid("0197c5c8-8112-732a-afc4-bf3764d966da"),
                Name = Role.User,
                NormalizedName = Role.User.ToUpper(),
                ConcurrencyStamp = "c9cc9ee2-620f-4e4f-adc5-bcddbfeb99f1"
            }
        );
        
    }
}

/*

System.InvalidOperationException: An error was generated for warning 'Microsoft.EntityFrameworkCore.Migrations.PendingModelChangesWarning': The model for context 'ApplicationDbContext' changes each time it is built. 
This is usually caused by dynamic values used in a 'HasData' call (e.g. `new DateTime()`, `Guid.NewGuid()`). 
Add a new migration and examine its contents to locate the cause, and replace the dynamic call with a static, hardcoded value. See
https://aka.ms/efcore-docs-pending-changes. This exception can be suppressed or logged by passing event ID 'RelationalEventId.PendingModelChangesWarning' 
to the 'ConfigureWarnings' method in 'DbContext.OnConfiguring' or 'AddDbContext'.

 */