using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mappings;
using Movies.Api.Routes;
using Movies.Application.Models.Queries.MovieQueries;
using Movies.Application.Services.MovieService;
using Movies.Contracts.Requests.MovieRequest;
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
        var query = new GetMovieByIdQuery { Id = id };
        var movieDto = await movieService.GetByIdAsync(query, cancellationToken);

        if (movieDto is null)
        {
            return NotFound();
        }

        var response = movieDto.ToMovieResponse();
        return Ok(response);
    }


    [HttpPost(MoviesEndpoints.Create)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken cancellationToken)
    {
        var command = request.ToCreateMovieCommand();
        var movieId = await movieService.CreateAsync(command, cancellationToken);

        if (movieId is null)
        {
            return BadRequest();
        }

        return CreatedAtAction(nameof(Get), new { id = movieId.Value }, null);
    }
    
}