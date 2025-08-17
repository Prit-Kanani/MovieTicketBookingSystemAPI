using FluentValidation;
using Movie_Management_API.Models;

namespace Movie_Management_API.Validators
{
    public class MovieValidator : AbstractValidator<Movie>
    {
        public MovieValidator() 
        {
            RuleFor(movie => movie.Name)
                .NotEmpty().WithMessage("Movie name is required.")
                .MaximumLength(100).WithMessage("Movie name cannot exceed 100 characters.");

            RuleFor(movie => movie.Language)
                .NotEmpty().WithMessage("Language is required.")
                .MaximumLength(50).WithMessage("Language cannot exceed 50 characters.");

            RuleFor(movie => movie.Duration)
                .GreaterThan(0).WithMessage("Duration must be greater than 0.");

            RuleFor(movie => movie.Poster)
                .NotEmpty().WithMessage("Poster URL is required.")
                .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
                .WithMessage("Poster URL must be a valid absolute URL.");

            RuleFor(movie => movie.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        }
    }
}
              