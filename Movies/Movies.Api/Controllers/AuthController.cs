using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mappings;
using Movies.Api.Routes;
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
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]

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
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
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



}