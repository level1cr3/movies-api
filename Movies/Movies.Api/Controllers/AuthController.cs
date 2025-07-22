using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mappings;
using Movies.Api.Routes;
using Movies.Application.Features.Auth.Constants;
using Movies.Application.Features.Auth.Services;
using Movies.Contracts.Requests.Auth;
using Movies.Contracts.Responses;
using Movies.Contracts.Responses.Auth;

namespace Movies.Api.Controllers;

[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
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
    }

    
    
    [HttpPost(AuthEndpoints.Login)]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<AppProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request.Email, request.Password, cancellationToken);

        if (result.IsFailure)
        {
            var appProblemDetails = result.AppErrors.ToAppProblemDetails(HttpContext);
            return BadRequest(appProblemDetails);
        }

        var tokenResponse = result.Value.ToTokenResponse();
        return Ok(tokenResponse);
    }

    
    [HttpPost(AuthEndpoints.RefreshToken)]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<AppProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.RefreshTokenAsync(request.Token, cancellationToken);
        
        if (result.IsFailure)
        {
            var appProblemDetails = result.AppErrors.ToAppProblemDetails(HttpContext);
            return BadRequest(appProblemDetails);
        }
        
        var tokenResponse = result.Value.ToTokenResponse();
        return Ok(tokenResponse);
    }


    [Authorize]
    [HttpPost(AuthEndpoints.Logout)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<AppProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.LogoutAsync(request.RefreshToken,cancellationToken);

        if (result.IsFailure)
        {
            var appProblemDetails = result.AppErrors.ToAppProblemDetails(HttpContext);
            return BadRequest(appProblemDetails);
        }

        return Ok();
    }

    
    
    // TODO : Forgot password
    
    
    
    
    // TODO : Reset password
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