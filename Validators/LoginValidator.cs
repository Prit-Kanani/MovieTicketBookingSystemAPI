using FluentValidation;
using Movie_Management_API.DTOs;

namespace Movie_Management_API.Validators
{
    public class LoginValidator : AbstractValidator<LoginDTO>
    {
        public LoginValidator() 
        {
            RuleFor(login => login.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(login => login.Password)
                .NotEmpty().WithMessage("Password is required.")
                .Matches(@"^[a-zA-Z0-9\s@#\$%&\-_]+$").WithMessage("Genre name can contain letters, numbers, spaces, and symbols like @, #, $, %, &, -, _.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .MaximumLength(100).WithMessage("Password cannot exceed 100 characters.");
        }
    }   
}
