using FluentValidation;
using Movies.Application.Models;
using System.Linq;


namespace Movies.Application.Validators;

public class GetAllMoviesOptionsValidator : AbstractValidator<GetAllMoviesOptions>
{
    public GetAllMoviesOptionsValidator()
    {
        RuleFor(x => x.Year).LessThanOrEqualTo(DateTime.UtcNow.Year);

        RuleFor(x => x.SortField)
            .Must(x => x is null ||
            (string.Equals(x, "title", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(x, "yearOfRelease", StringComparison.OrdinalIgnoreCase)))
            .WithMessage("You can only sort by title or yearOfRelease");

        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize).InclusiveBetween(1, 25)
            .WithMessage("Page size should be between 1 and 25");

    }
}
