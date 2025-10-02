using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mappings;
using Movies.Api.Routes;
using Movies.Application.Features.UserManagement.Services;
using Movies.Contracts.Responses;
using Movies.Contracts.Responses.Auth;

namespace Movies.Api.Controllers;

[ApiController]
public class UsersController(IUserService userService) : ControllerBase
{
    [Authorize]
    [HttpGet(UsersEndpoints.Me)]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<AppProblemDetails>(StatusCodes.Status400BadRequest)]
    // [ProducesResponseType(StatusCodes.Status403Forbidden)] // use this when authorize have the role or permission
    public async Task<IActionResult> GetCurrentUser()
    {
        var result = await userService.GetCurrentUserInfoAsync();
        
        if (result.IsFailure)
        {
            var appProblemDetails = result.AppErrors.ToAppProblemDetails(HttpContext);
            return BadRequest(appProblemDetails);
        }

        var userResponse = result.Value.ToUserResponse();
        return Ok(userResponse);
    }

}