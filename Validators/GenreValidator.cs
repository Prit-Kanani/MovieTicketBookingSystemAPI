using FluentValidation;
using Movie_Management_API.Models;

namespace Movie_Management_API.Validators
{
    public class GenreValidator : AbstractValidator<Genre>
    {
        public GenreValidator() 
        {
            RuleFor(genre => genre.Name)
                .NotEmpty().WithMessage("Genre name is required.")
                .MaximumLength(50).WithMessage("Genre name cannot exceed 50 characters.");
        }
    }
}
