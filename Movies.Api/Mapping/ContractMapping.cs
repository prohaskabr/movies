using Movies.Application.Models;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Mapping;

public static class ContractMapping
{
    public static Movie MapToMovie(this CreateMovieRequest request)
    {
        return new Movie()
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            YearOfRelease = request.YearOfRelease,
            Genres = request.Genres.ToList()
        };
    }

    public static Movie MapToMovie(this UpdateMovieRequest request, Guid id)
    {
        return new Movie()
        {
            Id = id,
            Title = request.Title,
            YearOfRelease = request.YearOfRelease,
            Genres = request.Genres.ToList()
        };
    }

    public static MovieResponse MapToMovieResponse(this Movie movie)
    {
        return new MovieResponse()
        {
            Id = movie.Id,
            Title = movie.Title,
            Slug = movie.Slug,
            YearOfRelease = movie.YearOfRelease,
            Genres = movie.Genres,
            UserRating = movie.UserRating,
            Rating = movie.Rating,
        };
    }

    public static MoviesResponse MapToMoviesResponse(this IEnumerable<Movie> movies,
        int page, int pageSize, int total)
    {
        return new MoviesResponse
        {
            Items = movies.Select(MapToMovieResponse),
            Page = page,
            PageSize = pageSize,
            Total = total
        };
    }

    public static MovieRatingResponse MapToRatingResponse(this MovieRating movieRating)
    {
        return new MovieRatingResponse()
        {
            MovieId = movieRating.MovieId,
            Slug = movieRating.Slug,
            Rating = movieRating.Rating,
        };
    }

    public static IEnumerable<MovieRatingResponse> MapToRatingsResponse(this IEnumerable<MovieRating> rating)
    {
        return rating.Select(x => x.MapToRatingResponse()).ToArray();
    }

    public static GetAllMoviesOptions MapToOptions(this GetAllMoviesRequest request)
    {
        return new GetAllMoviesOptions
        {
            Title = request.Title,
            Year = request.Year,
            SortField = request.SortBy?.Trim('+', '-'),
            SortOrder = request.SortBy is null ? SortOrder.Unsorted : request.SortBy.StartsWith('-') ? SortOrder.Descending : SortOrder.Ascending,
            Page = request.Page,
            PageSize = request.PagedSize
        };
    }
    public static GetAllMoviesOptions WithUserId(this GetAllMoviesOptions options, Guid? userId)
    {

        options.UserId = userId;
        return options;
    }
}
