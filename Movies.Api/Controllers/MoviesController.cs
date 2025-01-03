using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mapping;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers;

[ApiController]
public class MoviesController(IMovieRepository movieRepository) : ControllerBase
{

    [HttpPost(ApiEndpoints.Movies.Create)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request)
    {
        var movie = request.MapToMovie();

        await movieRepository.CreateAsync(movie);

        return Created($"/{ApiEndpoints.Movies.Create}/{movie.Id}", movie.MapToMovieResponse());
    }

    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var movie = await movieRepository.GetByIdAsync(id);

        if (movie is null)
            return NotFound();

        return Ok(movie.MapToMovieResponse());
    }

    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAll()
    {
        var movies = await movieRepository.GetAllAsync();

        return Ok(movies.MapToMoviesResponse());
    }
}
