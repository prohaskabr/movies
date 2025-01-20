using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;

namespace Movies.Api.V2.Controllers;

[ApiController]
[ApiVersion(2.0)]
public class MoviesController(IMovieService movieService) : ControllerBase
{

    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug, CancellationToken token)
    {
        var userId = HttpContext.GetUserId();

        var movie = Guid.TryParse(idOrSlug, out var id) ?
            await movieService.GetByIdAsync(id, userId, token) :
            await movieService.GetBySlugAsync(idOrSlug, userId, token);

        if (movie is null)
            return NotFound();

        return Ok(movie.MapToMovieResponse());
    }  
}
