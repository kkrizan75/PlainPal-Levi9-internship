using FluentValidation;
using PlanePal.DTOs.User;

namespace PlanePal.Validators
{
    public class LoginUserValidator : AbstractValidator<LoginUserDTO>
    {
        public LoginUserValidator()
        {
            RuleFor(l => l.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("A valid email format is required.");

            RuleFor(l => l.Password)
                .NotEmpty()
                .WithMessage("Password cannot be empty.");
        }
    }
}