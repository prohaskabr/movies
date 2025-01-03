using Movies.Application.Models;

namespace Movies.Application.Repositories;

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

    public Task<IEnumerable<Movie>> GetAllAsync()
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