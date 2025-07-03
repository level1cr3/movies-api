using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mappings;
using Movies.Api.Routes;
using Movies.Application.Services.Auth;
using Movies.Contracts.Requests.Auth;

namespace Movies.Api.Controllers;

[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    
    [HttpPost(AuthEndpoints.Register)]
    [ProducesResponseType(StatusCodes.Status201Created)]

    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var registerDto = request.ToRegisterDto();
        await authService.RegisterAsync(registerDto);
        return Created();
    }
    
    

}