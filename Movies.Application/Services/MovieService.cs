using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class MovieService(IMovieRepository repository, IValidator<Movie> movieValidator) : IMovieService
{

    public async Task<bool> CreateAsync(Movie movie)
    {
        await movieValidator.ValidateAndThrowAsync(movie);

        return await repository.CreateAsync(movie);
    }
    public async Task<Movie?> UpdateAsync(Movie movie)
    {
        await movieValidator.ValidateAndThrowAsync(movie);

        var exists = await repository.ExistsByIdAsync(movie.Id);

        if (!exists)
            return null;

        await repository.UpdateAsync(movie);

        return movie;
    }

    public Task<bool> DeleteByIdAsync(Guid id)
    {
        return repository.DeleteByIdAsync(id);
    }

    public Task<bool> ExistsByIdAsync(Guid id)
    {
        return repository.ExistsByIdAsync(id);
    }

    public Task<IEnumerable<Movie>> GetAllAsync()
    {
        return repository.GetAllAsync();
    }

    public Task<Movie?> GetByIdAsync(Guid id)
    {
        return repository.GetByIdAsync(id);
    }

    public Task<Movie?> GetBySlugAsync(string slug)
    {
        return repository.GetBySlugAsync(slug);
    }
}
