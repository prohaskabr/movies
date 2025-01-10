using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class MovieService(IMovieRepository repository, IValidator<Movie> movieValidator) : IMovieService
{

    public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
    {
        await movieValidator.ValidateAndThrowAsync(movie, token);

        return await repository.CreateAsync(movie, token);
    }
    public async Task<Movie?> UpdateAsync(Movie movie, CancellationToken token = default)
    {
        await movieValidator.ValidateAndThrowAsync(movie, token);

        var exists = await repository.ExistsByIdAsync(movie.Id, token);

        if (!exists)
            return null;

        await repository.UpdateAsync(movie, token);

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

    public Task<IEnumerable<Movie>> GetAllAsync(CancellationToken token = default)
    {
        return repository.GetAllAsync(token);
    }

    public Task<Movie?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        return repository.GetByIdAsync(id, token);
    }

    public Task<Movie?> GetBySlugAsync(string slug, CancellationToken token = default)
    {
        return repository.GetBySlugAsync(slug, token);
    }
}
