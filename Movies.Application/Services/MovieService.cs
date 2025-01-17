using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class MovieService(IMovieRepository repository, IRatingRepository ratingRepository, IValidator<Movie> movieValidator, IValidator<GetAllMoviesOptions> getAllMoviesValidator) : IMovieService
{

    public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
    {
        await movieValidator.ValidateAndThrowAsync(movie, token);

        return await repository.CreateAsync(movie, token);
    }
    public async Task<Movie?> UpdateAsync(Movie movie, Guid? userId, CancellationToken token = default)
    {
        await movieValidator.ValidateAndThrowAsync(movie, token);

        var exists = await repository.ExistsByIdAsync(movie.Id, token);

        if (!exists)
            return null;

        await repository.UpdateAsync(movie, token);

        if (!userId.HasValue)
        {
            var rating = await ratingRepository.GetRatingAsync(movie.Id, token);
            movie.Rating = rating;
            return movie;
        }

        var ratings = await ratingRepository.GetRatingAsync(movie.Id, userId.Value, token);
        movie.Rating = ratings.Rating;
        movie.UserRating = ratings.UserRating;

        return movie;
    }

    public Task<bool> DeleteByIdAsync(Guid id, CancellationToken token)
    {
        return repository.DeleteByIdAsync(id, token);
    }

    public Task<bool> ExistsByIdAsync(Guid id, CancellationToken token)
    {
        return repository.ExistsByIdAsync(id, token);
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options, CancellationToken token = default)
    {
        await getAllMoviesValidator.ValidateAndThrowAsync(options, token);

        return await repository.GetAllAsync(options, token);
    }

    public Task<Movie?> GetByIdAsync(Guid id, Guid? userId, CancellationToken token = default)
    {
        return repository.GetByIdAsync(id, userId, token);
    }

    public Task<Movie?> GetBySlugAsync(string slug, Guid? userId, CancellationToken token = default)
    {
        return repository.GetBySlugAsync(slug, userId, token);
    }

    public Task<int> GetCountByAsync(string? title, int? year, CancellationToken token = default)
    {
        return repository.GetCountByAsync(title, year, token);
    }
}
