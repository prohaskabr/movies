using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Validators;

public class MovieValidator : AbstractValidator<Movie>
{
    private readonly IMovieRepository movieRepository;
    public MovieValidator(IMovieRepository movieRepository)
    {
        this.movieRepository = movieRepository;

        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Genres).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.YearOfRelease).LessThanOrEqualTo(DateTime.UtcNow.Year);
        RuleFor(x => x.Slug).MustAsync(ValidateSlug).WithMessage("This movie already exists in the system");
    }

    private async Task<bool> ValidateSlug(Movie movie, string slug, CancellationToken token)
    {
        var existingMovie = await movieRepository.GetBySlugAsync(slug);

        //Updating
        if (existingMovie != null)
        {
            return existingMovie.Id == movie.Id;
        }

        return true;
    }
}
