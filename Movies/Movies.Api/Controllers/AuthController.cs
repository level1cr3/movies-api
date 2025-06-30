using Microsoft.AspNetCore.Mvc;
using Movies.Api.Routes;
using Movies.Contracts.Requests.Auth;

namespace Movies.Api.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    
    [HttpPost(AuthEndpoints.Register)]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        throw new NotImplementedException();
    }

}