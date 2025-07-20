using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Movies.Application.Data;
using Movies.Application.Data.Entities;
using Movies.Application.Data.Repositories;
using Movies.Application.Data.Repositories.JwtRefreshToken;
using Movies.Application.Email;
using Movies.Application.Features.Auth.Constants;
using Movies.Application.Features.Auth.DTOs;
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
    IValidator<RegisterDto> registerValidator,
    SignInManager<ApplicationUser> signInManager,
    IJwtTokenGenerator jwtTokenGenerator,
    IRefreshTokenRepository refreshTokenRepository,
    IRequestContextService requestContextService,
    IUnitOfWork unitOfWork) : IAuthService
{
    private readonly FrontendSettings _frontendSettings = optionsFrontend.Value;

    public async Task<Result> RegisterAsync(RegisterDto register)
    {
        var validationResult = await registerValidator.ValidateAsync(register);

        if (!validationResult.IsValid)
        {
            return Result.Failure(validationResult.Errors.ToAppErrors());
        }


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

            var createResult = await userManager.CreateAsync(user, register.Password);

            if (!createResult.Succeeded)
            {
                return Result.Failure(createResult.Errors.ToAppErrors());
            }

            var roleResult = await userManager.AddToRoleAsync(user, Role.User);

            if (!roleResult.Succeeded)
            {
                return Result.Failure(roleResult.Errors.ToAppErrors());
            }


            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl =
                $"{_frontendSettings.BaseUrl.TrimEnd('/')}{_frontendSettings.EmailConfirmationPath}?userId={Uri.EscapeDataString(user.Id.ToString())}&token={Uri.EscapeDataString(token)}";

            await emailService.SendAsync(user.Email, "Verify your email",
                $"""Please verify your account by clicking : <a href="{callbackUrl}" target="_blank">here</a>""");

            await transaction.CommitAsync();

            return Result.Success();
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

    public async Task<Result<AuthTokenDto>> LoginAsync(string email, string password)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return Result.Failure<AuthTokenDto>([LoginErrors.Invalid]);
        }

        var roles = await userManager.GetRolesAsync(user);
        var signInResult = await signInManager.PasswordSignInAsync(user, password, false, true);

        if (!signInResult.Succeeded)
        {
            if (signInResult.IsNotAllowed)
            {
                return Result.Failure<AuthTokenDto>([LoginErrors.NotAllowed]);
            }

            if (signInResult.IsLockedOut)
            {
                return Result.Failure<AuthTokenDto>([LoginErrors.LockedOut]);
            }

            return Result.Failure<AuthTokenDto>([LoginErrors.Invalid]);
        }

        var authTokenDto = await jwtTokenGenerator.GenerateTokenAsync(user, roles);
        return Result.Success(authTokenDto);
    }

    public async Task<Result<AuthTokenDto>> RefreshTokenAsync(string token)
    {
        var dbRefreshToken = await refreshTokenRepository.GetByRefreshTokenAsync(token);
        var clientIp = requestContextService.GetClientIp();

        if (dbRefreshToken is null)
        {
            return Result.Failure<AuthTokenDto>([RefreshTokenErrors.Invalid]);
        }

        if (dbRefreshToken.IsRevoked)
        {
            await refreshTokenRepository
                .RevokeAsync(dbRefreshToken.Id, RefreshTokenRevokeReason.ReuseDetected, clientIp);
            
            return Result.Failure<AuthTokenDto>([RefreshTokenErrors.Invalid]);
        }

        var roles = await userManager.GetRolesAsync(dbRefreshToken.User);
        var authToken = await jwtTokenGenerator.GenerateTokenAsync(dbRefreshToken.User, roles);
        
        await refreshTokenRepository
            .RevokeAsync(dbRefreshToken.Id, RefreshTokenRevokeReason.TokenRotated, clientIp,authToken.RefreshTokenId);
        
        return Result.Success(authToken);
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