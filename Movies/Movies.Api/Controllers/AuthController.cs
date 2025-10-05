using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Movies.Api.Mappings;
using Movies.Api.Routes;
using Movies.Api.Services;
using Movies.Api.Settings;
using Movies.Application.Features.Auth.Constants;
using Movies.Application.Features.Auth.DTOs;
using Movies.Application.Features.Auth.Services;
using Movies.Application.Shared.Foundation;
using Movies.Contracts.Requests.Auth;
using Movies.Contracts.Responses;
using Movies.Contracts.Responses.Auth;

namespace Movies.Api.Controllers;

[ApiController]
public class AuthController(IAuthService authService, IRefreshTokenCookieService refreshTokenCookieService) : ControllerBase
{
    
    [HttpPost(AuthEndpoints.Register)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<AppProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var registerDto = request.ToRegisterDto();
        var result = await authService.RegisterAsync(registerDto, cancellationToken);

        if (result.IsFailure)
        {
            var appProblemDetails = result.AppErrors.ToAppProblemDetails(HttpContext);
            return BadRequest(appProblemDetails);
        }
        
        return Created();
    }
    
    
    [HttpPost(AuthEndpoints.ConfirmEmail)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<AppProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.ConfirmEmailAsync(request.UserId, request.Token, cancellationToken);

        if (result.IsFailure)
        {
            var appProblemDetails = result.AppErrors.ToAppProblemDetails(HttpContext);
            return BadRequest(appProblemDetails);
        }

        return Ok();
    }

    
    [HttpPost(AuthEndpoints.ResendConfirmEmail)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ResendConfirmEmail([FromBody] ResendConfirmationEmailRequest request, CancellationToken cancellationToken)
    {
        // add rate limiting via rate limiter and also add the captcha. for captcha create separate endpoint
        // don't send captcha value with every object.
        
        await authService.ResendConfirmEmailAsync(request.Email, cancellationToken);
        return Ok();
        
        // no bad request etc to prevent user enumeration attacks
    }

    
    
    [HttpPost(AuthEndpoints.Login)]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<AppProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request.Email, request.Password, cancellationToken);

        if (result.IsFailure)
        {
            var appProblemDetails = result.AppErrors.ToAppProblemDetails(HttpContext);
            return BadRequest(appProblemDetails);
        }

        var authToken = result.Value.AuthTokenDto;
        refreshTokenCookieService.SendRefreshToken(authToken.RefreshToken,authToken.RefreshTokenExpiresAt);
        
        var loginResponse = result.Value.ToLoginResponse();
        return Ok(loginResponse);
    }

    
    [HttpPost(AuthEndpoints.RefreshToken)]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
    {
        var refreshToken= refreshTokenCookieService.GetRefreshToken();

        if (refreshToken is null)
        {
            return Unauthorized();
        }
        
        var result = await authService.RefreshTokenAsync(refreshToken, cancellationToken);
        
        if (result.IsFailure)
        {
            return Unauthorized();
        }
        
        var authToken = result.Value;
        refreshTokenCookieService.SendRefreshToken(authToken.RefreshToken,authToken.RefreshTokenExpiresAt);
        var tokenResponse = authToken.ToTokenResponse();
        return Ok(tokenResponse);
    }


    [Authorize]
    [HttpPost(AuthEndpoints.Logout)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<AppProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        //TODO : during logout how can i get refresh token from cookie ?. when it is bound to only refresh token path ?
        var refreshToken= refreshTokenCookieService.GetRefreshToken();

        if (refreshToken is null)
        {
            refreshTokenCookieService.DeleteRefreshToken();
            return Ok();
        }
        
        var result = await authService.LogoutAsync(refreshToken,cancellationToken);
        refreshTokenCookieService.DeleteRefreshToken();
        
        if (result.IsFailure)
        {
            var appProblemDetails = result.AppErrors.ToAppProblemDetails(HttpContext);
            return BadRequest(appProblemDetails);
        }

        return Ok();
    }


    [HttpPost(AuthEndpoints.ForgotPassword)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        await authService.ForgotPasswordAsync(request.Email, cancellationToken);
        return Ok(); // no bad request etc to prevent user enumeration attacks
    }



    [HttpPost(AuthEndpoints.ResetPassword)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<AppProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.ResetPasswordAsync(request.UserId,request.Token,request.NewPassword,cancellationToken);

        if (result.IsFailure)
        {
            var appProblemDetails = result.AppErrors.ToAppProblemDetails(HttpContext);
            return BadRequest(appProblemDetails);
        }
        
        return Ok();
    }
    
    
    
    
    // Todo later
    // TODO : Change Email
    // TODO : 2 factor auth
    // TODO : OAuth
    
    

    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    // Todo : Delete later where created only for testing.
    [Authorize]
    [HttpGet("auth-test")]
    public IActionResult Method()
    {
        return Ok(User);
    }
    
    [Authorize(Roles = Role.Admin)]
    [HttpGet("admin-test")]
    public IActionResult Method2()
    {
        return Ok(User);
    }

    
}