using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers;

[ApiController]
public class RatingsController(IRatingService service) : ControllerBase
{

    [Authorize]
    [HttpPut(ApiEndpoints.Movies.Rate)]
    public async Task<IActionResult> RateMovie([FromRoute] Guid id, [FromBody] RateMovieRequest request, CancellationToken token)
    {
        var userId = HttpContext.GetUserId();

        var result = await service.RateMovieAsync(id, request.Rating, userId!.Value, token);

        return result ? Ok() : NotFound();
    }

    [Authorize]
    [HttpDelete(ApiEndpoints.Movies.DeleteRate)]
    public async Task<IActionResult> DeleteMovie([FromRoute] Guid id, CancellationToken token)
    {
        var userId = HttpContext.GetUserId();

        var result = await service.DeleteRatingAsync(id, userId!.Value, token);

        return result ? Ok() : NotFound();
    }

    [Authorize]
    [HttpGet(ApiEndpoints.Ratings.GetUserRatings)]
    public async Task<IActionResult> GetUserRatings(CancellationToken token)
    {
        var userId = HttpContext.GetUserId();

        var result = await service.GetRatingsForUserAsync(userId!.Value, token);

        return  Ok(result.MapToRatingsResponse());
    }
}
