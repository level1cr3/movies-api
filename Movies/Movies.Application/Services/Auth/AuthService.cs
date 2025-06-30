using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Movies.Application.Constants;
using Movies.Application.Data.Entities;
using Movies.Application.DTOs.Auth;
using Movies.Application.Email;

namespace Movies.Application.Services.Auth;

internal class AuthService(UserManager<ApplicationUser> userManager, IEmailService emailService)
{
    public async Task<bool> Register(RegisterDto register)
    {
        // validate register Dto using fluent validation.

        var user = new ApplicationUser
        {
            FirstName = register.FirstName,
            LastName = register.LastName,
            Email = register.Email
        };

        var result = await userManager.CreateAsync(user, register.Password);

        if (!result.Succeeded)
        {
            var validationFailures = result.Errors
                .Select(error => new ValidationFailure(error.Code, error.Description)).ToList();

            throw new ValidationException(validationFailures);
        }


        var result2 = await userManager.AddToRoleAsync(user, Role.User);

        if (!result2.Succeeded)
        {
            var validationFailures = result.Errors
                .Select(error => new ValidationFailure(error.Code, error.Description)).ToList();

            throw new ValidationException(validationFailures);
        }
        
        
        // send confirmation email.
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var callbackUrl = $"";

        await emailService.SendAsync(user.Email, "Verify your email", 
            $"""Please verify your account by clicking : <a href="{callbackUrl}" target="_blank">here</a>""");

        return true;
    }
}


/*
 # Why i don't have repository wrapping user manager,signing manager and role manager.

 🔄 Identity Is the Exception
ASP.NET Identity (via UserManager, RoleManager, SignInManager) already encapsulates the
repository pattern and has a rich API — so you usually don’t wrap it unless you have a reason.


📌 When Would You Create a Repository Around UserManager?
You might do that only if:

You want to decouple your service from ASP.NET Identity to make it easier to swap out in the future.

You need to add custom query methods that UserManager doesn’t provide.

You are implementing unit tests and want to mock a simpler abstraction.

You're building a domain-driven design (DDD) style system, where you're wrapping EF-related services behind a domain-specific repository interface.

 */