using FluentValidation;
using Movie_Management_API.Models;

namespace Movie_Management_API.Validators
{
    public class TheatreValidator : AbstractValidator<Theatre>
    {
        public TheatreValidator() 
        {
            RuleFor(theatre => theatre.Name)
                .NotEmpty().WithMessage("Theatre name is required.")
                .MaximumLength(100).WithMessage("Theatre name cannot exceed 100 characters.");

            RuleFor(theatre => theatre.Address)
                .MaximumLength(200).WithMessage("Address cannot exceed 200 characters.")
                .When(theatre => !string.IsNullOrEmpty(theatre.Address));

            RuleFor(theatre => theatre.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(50).WithMessage("City cannot exceed 50 characters.");

            RuleFor(theatre => theatre.UserId)
                .GreaterThan(0).WithMessage("User ID must be greater than 0.")
                .WithMessage("A valid User ID is required for the theatre.");

            RuleFor(theatre => theatre.Screens)
                .NotEmpty().WithMessage("At least one screen is required for the theatre.")
                .WithMessage("Theatre must have at least one screen associated with it.");
            
        }
    }
}
