using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mappings;
using Movies.Api.Routes;
using Movies.Application.Features.Auth.Constants;
using Movies.Application.Features.Auth.Services;
using Movies.Contracts.Requests.Auth;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers;

[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    
    [HttpPost(AuthEndpoints.Register)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<AppProblemDetails>(StatusCodes.Status400BadRequest)]

    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var registerDto = request.ToRegisterDto();
        var result = await authService.RegisterAsync(registerDto);

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
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
    {
        var result = await authService.ConfirmEmailAsync(request.UserId, request.Token);

        if (result.IsFailure)
        {
            var appProblemDetails = result.AppErrors.ToAppProblemDetails(HttpContext);
            return BadRequest(appProblemDetails);
        }

        return Ok();
    }



    [HttpPost(AuthEndpoints.Login)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<AppProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await authService.LoginAsync(request.Email, request.Password);

        if (result.IsFailure)
        {
            var appProblemDetails = result.AppErrors.ToAppProblemDetails(HttpContext);
            return BadRequest(appProblemDetails);
        }

        var loginResponse = result.Value.ToLoginResponse();
        return Ok(loginResponse);
    }

    
        
    [Authorize]
    [HttpGet("auth-test")]
    public IActionResult Method()
    {
        var user = User;
        return Ok();
    }
    
    [Authorize(Roles = Role.Admin)]
    [HttpGet("admin-test")]
    public IActionResult Method2()
    {
        var user = User;
        return Ok();
    }

    
}