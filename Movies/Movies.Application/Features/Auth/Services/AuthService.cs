﻿using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Movies.Application.Data;
using Movies.Application.Data.Entities;
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
    IRequestContextService requestContextService) : IAuthService
{
    private readonly FrontendSettings _frontendSettings = optionsFrontend.Value;

    public async Task<Result> RegisterAsync(RegisterDto register, CancellationToken cancellationToken = default)
    {
        var validationResult = await registerValidator.ValidateAsync(register, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result.Failure(validationResult.Errors.ToAppErrors());
        }


        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

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

            await SendConfirmationEmailAsync(user);

            await transaction.CommitAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<Result> ConfirmEmailAsync(string userId, string token,
        CancellationToken cancellationToken = default)
    {
        userId = Uri.UnescapeDataString(userId);
        token = Uri.UnescapeDataString(token);

        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return Result.Failure([EmailConfirmationErrors.InvalidRequest]);
        }

        if (user.EmailConfirmed)
        {
            return Result.Success();
        }

        var confirmEmailResult = await userManager.ConfirmEmailAsync(user, token);

        return confirmEmailResult.Succeeded
            ? Result.Success()
            : Result.Failure(confirmEmailResult.Errors.ToAppErrors());
    }

    public async Task<Result> ResendConfirmEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        // { } is a property pattern that matches any non-null object.
        // if (await userManager.FindByEmailAsync(email) is not { } user || user.EmailConfirmed) 
        // {
        //     return Result.Success(); // return success to prevent user enumeration error.
        // }

        var user = await userManager.FindByEmailAsync(email);

        if (user is null || user.EmailConfirmed)
        {
            return Result.Success(); // return success to prevent user enumeration error.
        }

        await SendConfirmationEmailAsync(user);
        return Result.Success();
    }

    public async Task<Result<AuthTokenDto>> LoginAsync(string email, string password,
        CancellationToken cancellationToken = default)
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

        var authTokenDto = await jwtTokenGenerator.GenerateTokenAsync(user, roles, cancellationToken);
        return Result.Success(authTokenDto);
    }

    public async Task<Result<AuthTokenDto>> RefreshTokenAsync(string token,
        CancellationToken cancellationToken = default)
    {
        var dbRefreshToken = await refreshTokenRepository.GetByRefreshTokenAsync(token, cancellationToken);
        var clientIp = requestContextService.GetClientIp();

        if (dbRefreshToken is null)
        {
            return Result.Failure<AuthTokenDto>([RefreshTokenErrors.Invalid]);
        }

        if (dbRefreshToken.IsRevoked)
        {
            await refreshTokenRepository
                .RevokeAsync(dbRefreshToken.Id, RefreshTokenRevokeReason.ReuseDetected, clientIp,
                    cancellationToken: cancellationToken);

            return Result.Failure<AuthTokenDto>([RefreshTokenErrors.Invalid]);
        }

        var roles = await userManager.GetRolesAsync(dbRefreshToken.User);
        var authToken = await jwtTokenGenerator.GenerateTokenAsync(dbRefreshToken.User, roles, cancellationToken);

        await refreshTokenRepository
            .RevokeAsync(dbRefreshToken.Id, RefreshTokenRevokeReason.TokenRotated, clientIp, authToken.RefreshTokenId,
                cancellationToken);

        return Result.Success(authToken);
    }

    public async Task<Result> LogoutAsync(string token, CancellationToken cancellationToken = default)
    {
        var dbRefreshToken = await refreshTokenRepository.GetByRefreshTokenAsync(token, cancellationToken);
        var clientIp = requestContextService.GetClientIp();

        if (dbRefreshToken is null || dbRefreshToken.IsRevoked)
        {
            return Result.Failure([RefreshTokenErrors.Invalid]);
        }

        await refreshTokenRepository.RevokeAsync(dbRefreshToken.Id,
            RefreshTokenRevokeReason.UserLoggedOut, clientIp, cancellationToken: cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ForgotPasswordAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user is not null && user.EmailConfirmed)
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl =
                $"{_frontendSettings.BaseUrl.TrimEnd('/')}{_frontendSettings.ResetPasswordPath}?userId={Uri.EscapeDataString(user.Id.ToString())}&token={Uri.EscapeDataString(token)}";

            await emailService.SendAsync(user.Email!, "Reset password",
                $"""Please reset your password by clicking : <a href="{callbackUrl}" target="_blank">here</a>""");
        }

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(string userId, string token, string newPassword,
        CancellationToken cancellationToken = default)
    {
        userId = Uri.UnescapeDataString(userId);
        token = Uri.UnescapeDataString(token);

        var user = await userManager.FindByIdAsync(userId);

        if (user is null || !user.EmailConfirmed)
        {
            return Result.Failure([ResetPasswordTokenError.Invalid]);
        }

        var resetPasswordResult = await userManager.ResetPasswordAsync(user, token, newPassword);
        
        return resetPasswordResult.Succeeded
            ? Result.Success()
            : Result.Failure(resetPasswordResult.Errors.ToAppErrors());
    }


    // Need to implement outbox pattern for email sending.
    private async Task SendConfirmationEmailAsync(ApplicationUser user)
    {
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var callbackUrl =
            $"{_frontendSettings.BaseUrl.TrimEnd('/')}{_frontendSettings.EmailConfirmationPath}?userId={Uri.EscapeDataString(user.Id.ToString())}&token={Uri.EscapeDataString(token)}";

        await emailService.SendAsync(user.Email!, "Verify your email",
            $"""Please verify your account by clicking : <a href="{callbackUrl}" target="_blank">here</a>""");
    }
}


/*
 # Why I don't have repository wrapping user manager,signing manager and role manager.

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

But this is rare case I just want it to have it here.

### Because it will never happen because roles would have been seeded before only via migrations.





📌 When Would You Create a Repository Around UserManager?
You might do that only if:

You want to decouple your service from ASP.NET Identity to make it easier to swap out in the future.

You need to add custom query methods that UserManager does not provide.

You are implementing unit tests and want to mock a simpler abstraction.

You're building a domain-driven design (DDD) style system, where you're wrapping EF-related services behind a domain-specific repository interface.



# How would we make sure that user is logged in on single device at a time ?

Best Practice: On login, revoke existing tokens and issue new ones
   Before issuing new tokens, revoke all active refresh tokens for the user.

   Then generate a fresh refresh/access token pair for the new session.

   This ensures:

   Any previous device or session will no longer be able to use its refresh token to get new access tokens.

   Only the most recent login is allowed to stay active.



🧠 What to Revoke?
   You should revoke:

   All refresh tokens where:

   UserId == current user

   IsRevoked == false



public async Task RevokeAllActiveTokensAsync(Guid userId, string ip, string reason, CancellationToken cancellationToken = default)
   {
       var activeTokens = await db.RefreshTokens
           .Where(t => t.UserId == userId && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow)
           .ToListAsync(cancellationToken);

       foreach (var token in activeTokens)
       {
           token.IsRevoked = true;
           token.RevokedAt = DateTime.UtcNow;
           token.RevokedByIp = ip;
           token.RevokedReason = reason;
       }

       await db.SaveChangesAsync(cancellationToken);
   }


During login

await RevokeAllActiveTokensAsync(user.ID, ipAddress, "New Login", cancellationToken);
   var newRefreshToken = GenerateRefreshToken(...);
   // Save and return the new token

What's considered good enough in production?
   Most secure systems accept a short window of overlap, and do this:

   Access token TTL is short, e.g., 5–10 minutes.

   Refresh token is long-lived, but gets revoked on new login.

   Rotate refresh tokens with each use (helps detect theft).




*** Use iat (issued-at) in access tokens, and store lastLoginAt per user:

            You can reject tokens issued before lastLoginAt if you want to be more aggressive.

At login :
user.LastLoginAt = DateTime.UtcNow;

At API auth: I would need to create custom auth policy for all users or roles that would be applicable.

var tokenIssuedAt = jwtPayload.iat; // Unix timestamp
   if (tokenIssuedAt < user.LastLoginAt)
   {
       reject("Token issued before last login.");
   }

Now you can instantly expire all old tokens — but at the cost of a DB call on each request.




## for logout
*** I should blacklist jwt in memory for. time it is valid for like 15 minutes. but then it would require me
 to write custom middleware to check if jwt is blacklisted for every request. which then is kinda of bad.

 and if we are having to do all these then maybe we should consider moving from jwt to cookie based application
 and drop idea of api to begin with I think

I will allow multiple device logins. so I won't revoke all the tokens during login or logout.
only remove the one that user have but how will i know which one the user have ?
well I will send the refresh token for logout and revoke It. hence successful logout. else if token is already
revoked return bad request.


 */