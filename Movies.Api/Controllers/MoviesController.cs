using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers;

[ApiController]
public class MoviesController(IMovieService movieService) : ControllerBase
{

    [HttpPost(ApiEndpoints.Movies.Create)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request)
    {
        var movie = request.MapToMovie();

        await movieService.CreateAsync(movie);

        return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie);
    }

    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug)
    {
        var movie = Guid.TryParse(idOrSlug, out var id) ?
            await movieService.GetByIdAsync(id) :
            await movieService.GetBySlugAsync(idOrSlug);

        if (movie is null)
            return NotFound();

        return Ok(movie.MapToMovieResponse());
    }

    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAll()
    {
        var movies = await movieService.GetAllAsync();

        return Ok(movies.MapToMoviesResponse());
    }

    [HttpPut(ApiEndpoints.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request)
    {
        var movie = request.MapToMovie(id);

        var updatedMovie = await movieService.UpdateAsync(movie);

        if (updatedMovie is null)
            return NotFound();

        return Ok(updatedMovie.MapToMovieResponse());
    }

    [HttpDelete(ApiEndpoints.Movies.Delete)]
    public async Task<IActionResult> Felete([FromRoute] Guid id)
    {
        var deleted = await movieService.DeleteByIdAsync(id);

        if (!deleted)
            return NotFound();

        return Ok();
    }
}
