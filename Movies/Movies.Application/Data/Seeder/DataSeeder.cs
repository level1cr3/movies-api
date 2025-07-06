using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Movies.Application.Data.Entities;
using Movies.Application.Features.Auth.Constants;

namespace Movies.Application.Data.Seeder;

internal class DataSeeder(
    RoleManager<ApplicationRole> roleManager,
    UserManager<ApplicationUser> userManager,
    IOptions<AdminInfo> options,
    ILogger<DataSeeder> logger) : IDataSeeder
{
    private readonly AdminInfo _adminInfo = options.Value;

    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedAdminUserAsync();
    }


    private async Task SeedRolesAsync()
    {
        string[] roles = [Role.Admin, Role.User];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(new ApplicationRole(role));

                if (result.Succeeded)
                {
                    logger.LogInformation("Created role: {RoleName}", role);
                }
                else
                {
                    logger.LogError("Failed to create role: {RoleName}. Errors: {Errors}", role,
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }

    private async Task SeedAdminUserAsync()
    {
       var admin = await userManager.FindByEmailAsync(_adminInfo.Email);

       if (admin is not null)
       {
           return;
       }

       var adminUser = new ApplicationUser
       {
           FirstName = "AdminUser",
           Email = _adminInfo.Email,
           UserName = _adminInfo.Email,
           EmailConfirmed = true
       };

       var createResult = await userManager.CreateAsync(adminUser, _adminInfo.Password);

       if (createResult.Succeeded)
       {
           await userManager.AddToRoleAsync(adminUser, Role.Admin);
           logger.LogInformation("Created admin user: {Email}", adminUser.Email);
       }
       else
       {
           logger.LogError("Failed to create admin user. Errors: {Errors}", 
               string.Join(", ", createResult.Errors.Select(e => e.Description)));
       }
       
    }
    
    
}