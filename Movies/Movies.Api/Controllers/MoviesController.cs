using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mappings;
using Movies.Api.Routes;
using Movies.Application.Features.Movie.Services;
using Movies.Contracts.Requests.MovieRequest;
using Movies.Contracts.Responses;
using Movies.Contracts.Responses.MovieResponse;

namespace Movies.Api.Controllers;

[ApiController]
public class MoviesController(IMovieService movieService) : ControllerBase
{
    [HttpGet(MoviesEndpoints.Get)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<MovieResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await movieService.GetByIdAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound();
        }

        var response = result.Value.ToMovieResponse();
        return Ok(response);
    }


    [HttpPost(MoviesEndpoints.Create)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<AppProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken cancellationToken)
    {
        var createMovieDto = request.ToCreateMovieDto();
        var result = await movieService.CreateAsync(createMovieDto, cancellationToken);

        if (result.IsFailure)
        {
            var appProblemDetails = result.AppErrors.ToAppProblemDetails(HttpContext);
            return BadRequest(appProblemDetails);
        }

        return CreatedAtAction(nameof(Get), new { id = result.Value }, null);
    }
    
}