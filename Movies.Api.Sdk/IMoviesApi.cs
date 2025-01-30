using Movies.Contracts.Requests;
using Movies.Contracts.Responses;
using Refit;

namespace Movies.Api.Sdk;

[Headers("Authorization: Bearer")]
public interface IMoviesApi
{
    [Get(ApiEndpoints.Movies.Get)]
    Task<MovieResponse> GetMovieAsync(string idOrSlug);

    [Get(ApiEndpoints.Movies.GetAll)]
    Task<MoviesResponse> GetMoviesAsync(GetAllMoviesRequest request);

    [Post(ApiEndpoints.Movies.Create)]
    Task<MoviesResponse> CreateMovieAsync(CreateMovieRequest request);

    [Put(ApiEndpoints.Movies.Update)]
    Task<MoviesResponse> UpdateMovieAsync(Guid id, UpdateMovieRequest request);

    [Delete(ApiEndpoints.Movies.Delete)]
    Task<MoviesResponse> DeleteMovieAsync(Guid id);
}
