using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers;

[ApiController]
[ApiVersion(1.0)]
public class MoviesController(IMovieService movieService) : ControllerBase
{

    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpPost(ApiEndpoints.Movies.Create)]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken token)
    {
        var movie = request.MapToMovie();

        await movieService.CreateAsync(movie, token);

        return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie);
    }

    [HttpGet(ApiEndpoints.Movies.Get)]
    [ResponseCache(Duration = 30, VaryByHeader = "Accept, Accept-Encoding", Location =ResponseCacheLocation.Any)]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    [HttpGet(ApiEndpoints.Movies.GetAll)]
    [ResponseCache(Duration = 30, VaryByQueryKeys  = new[] { "title","year","sortBy","page", "pageSize"}, Location = ResponseCacheLocation.Any)]
    [ProducesResponseType(typeof(MoviesResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllMoviesRequest request, CancellationToken token)
    {
        var options = request.MapToOptions().WithUserId(HttpContext.GetUserId());

        var movies = await movieService.GetAllAsync(options, token);
        var moviesCount = await movieService.GetCountByAsync(options.Title, options.Year, token);

        return Ok(movies.MapToMoviesResponse(options.Page, options.PageSize, moviesCount));
    }

    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpPut(ApiEndpoints.Movies.Update)]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request, CancellationToken token)
    {
        var userId = HttpContext.GetUserId();

        var movie = request.MapToMovie(id);

        var updatedMovie = await movieService.UpdateAsync(movie, userId, token);

        if (updatedMovie is null)
            return NotFound();

        return Ok(updatedMovie.MapToMovieResponse());
    }

    [Authorize(AuthConstants.AdminUserPolicyName)]
    [HttpDelete(ApiEndpoints.Movies.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Felete([FromRoute] Guid id, CancellationToken token)
    {
        var deleted = await movieService.DeleteByIdAsync(id, token);

        if (!deleted)
            return NotFound();

        return Ok();
    }
}
