using System.Security.Cryptography.X509Certificates;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public interface IMovieRepository
{
    Task<bool> CreateAsync(Movie movie);
    Task<Movie?> GetByIdAsync(Guid id);
    Task<IEnumerable<Movie>> GetAllAsync(Guid id);
    Task<bool> UpdateAsync(Movie movie);
    Task<bool> DeleteByIdAsync(Guid id);
}

public class MovieRepository : IMovieRepository
{

    private readonly List<Movie> _repository = new();

    public Task<bool> CreateAsync(Movie movie)
    {
        _repository.Add(movie);
        return Task.FromResult(true);
    }

    public Task<Movie?> GetByIdAsync(Guid id)
    {
        var movie = _repository.FirstOrDefault(x => x.Id == id);
        return Task.FromResult(movie);
    }

    public Task<IEnumerable<Movie>> GetAllAsync(Guid id)
    {
        var movies = _repository.AsEnumerable();
        return Task.FromResult(movies);
    }

    public Task<bool> UpdateAsync(Movie movie)
    {
        var index = _repository.FindIndex(x => x.Id == movie.Id);

        if (index == -1)
            return Task.FromResult(false);

        _repository[index] = movie;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteByIdAsync(Guid id)
    {
        var count = _repository.RemoveAll(x => x.Id == id);

        return Task.FromResult(count > 0);
    }
}