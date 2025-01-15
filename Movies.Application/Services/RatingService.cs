
using FluentValidation;
using FluentValidation.Results;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class RatingService(IRatingRepository repository, IMovieRepository movieRepository) : IRatingService
{
    public async Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken token = default)
    {
        if (rating is <= 0 or > 5)
        {
            throw new ValidationException(new ValidationFailure[]
            {
                new ValidationFailure
                {
                PropertyName = "Rating",
                ErrorMessage = "Rating must be betweeb 1 and 5"
                }
            });
        }

        var exists = await movieRepository.ExistsByIdAsync(movieId);

        if (!exists)
            return false;

        return await repository.RateMovieAsync(movieId, rating, userId, token);
    }

    public async Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken token = default)
    {
        return await repository.DeleteRatingAsync(movieId, userId, token);
    }

    public async Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default)
    {
        return await repository.GetRatingsForUserAsync(userId, token);
    }
}
