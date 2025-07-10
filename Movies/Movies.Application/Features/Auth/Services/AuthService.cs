using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Movies.Application.Data;
using Movies.Application.Data.Entities;
using Movies.Application.DTOs.Auth;
using Movies.Application.Email;
using Movies.Application.Features.Auth.Constants;
using Movies.Application.Features.Auth.Errors;
using Movies.Application.Features.Auth.Mappings;
using Movies.Application.Settings;
using Movies.Application.Shared.Foundation;

namespace Movies.Application.Features.Auth.Services;

internal class AuthService(
    ApplicationDbContext db,
    UserManager<ApplicationUser> userManager,
    IEmailService emailService,
    IOptions<FrontendSettings> optionsFrontend,
    IValidator<RegisterDto> registerValidator) : IAuthService
{
    private readonly FrontendSettings _frontendSettings = optionsFrontend.Value;

    public async Task RegisterAsync(RegisterDto register)
    {

        var validationResult = await registerValidator.ValidateAsync(register);

        await registerValidator.ValidateAndThrowAsync(register);
        
        await using var transaction = await db.Database.BeginTransactionAsync();

        try
        {
            var user = new ApplicationUser
            {
                FirstName = register.FirstName,
                LastName = register.LastName,
                Email = register.Email,
                UserName = register.Email,
            };

            var result = await userManager.CreateAsync(user, register.Password);
            ThrowIfFailed(result);
            result = await userManager.AddToRoleAsync(user, Role.User);
            ThrowIfFailed(result);

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl =
                $"{_frontendSettings.BaseUrl.TrimEnd('/')}{_frontendSettings.EmailConfirmationPath}?userId={Uri.EscapeDataString(user.Id.ToString())}&token={Uri.EscapeDataString(token)}";

            await emailService.SendAsync(user.Email, "Verify your email",
                $"""Please verify your account by clicking : <a href="{callbackUrl}" target="_blank">here</a>""");

            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Result> ConfirmEmailAsync(string userId, string token)
    {
        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return Result.Failure([EmailConfirmationErrors.InvalidRequest]);
        }

        if (user.EmailConfirmed)
        {
            return Result.Success();
        }

        var result = await userManager.ConfirmEmailAsync(user, token);

        return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.ToAppErrors());
    }


    private static void ThrowIfFailed(IdentityResult result)
    {
        if (result.Succeeded) return;

        var validationFailures = result.Errors
            .Select(error => new ValidationFailure(error.Code, error.Description));

        throw new ValidationException(validationFailures);
    }
}


/*
 # Why i don't have repository wrapping user manager,signing manager and role manager.

 🔄 Identity Is the Exception
    ASP.NET Identity (via UserManager, RoleManager, SignInManager) already encapsulates the
    repository pattern and has a rich API — so you usually don’t wrap it unless you have a reason.

❗ Exception: Identity Operations
    Identity methods like:

    UserManager.CreateAsync()
    UserManager.AddToRoleAsync()
    RoleManager.CreateAsync()

    These internally save changes (they call SaveChangesAsync() under the hood), so:
    They bypass your Unit of Work flow.
    That’s why you need explicit transaction control (like you’re doing with BeginTransactionAsync()).

But this is rare case i just want it to have it here.

### Because it will never happen because roles would have been seeded before only via migrations.





📌 When Would You Create a Repository Around UserManager?
You might do that only if:

You want to decouple your service from ASP.NET Identity to make it easier to swap out in the future.

You need to add custom query methods that UserManager doesn’t provide.

You are implementing unit tests and want to mock a simpler abstraction.

You're building a domain-driven design (DDD) style system, where you're wrapping EF-related services behind a domain-specific repository interface.

 */