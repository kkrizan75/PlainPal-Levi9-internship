using FluentValidation;
using PlanePal.DTOs.User;

namespace PlanePal.Validators
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserDTO>
    {
        public UpdateUserValidator()
        {
            RuleFor(u => u.FirstName)
                .NotEmpty()
                .Matches(@"^[A-Za-z-\s]*$")
                .WithMessage("Name should only contain letters.")
                .Length(2, 50);
            RuleFor(u => u.LastName)
                .NotEmpty()
                .Matches(@"^[A-Za-z-\s]*$")
                .WithMessage("Surname should only contain letters.")
                .Length(2, 50);
            RuleFor(u => u.PhoneNumber)
                .NotEmpty()
                .Matches(@"^[+]{1}[0-9]+$")
                .WithMessage("Phone number should be in format '+1234'.");
            RuleFor(u => u.DateOfBirth)
                .NotNull()
                .Must(BeAValidDate)
                .WithMessage("Age must be in the range 16-120.");
            RuleFor(u => u.City)
                .NotEmpty()
                .Matches(@"^[A-Za-z\s]*$")
                .WithMessage("City should only contain letters.");
            RuleFor(u => u.Country)
                .NotEmpty()
                .Matches(@"^[A-Za-z\s]*$")
                .WithMessage("Country should only contain letters.");
            RuleFor(u => u.Street)
                .NotEmpty()
                .Matches(@"^[A-Za-z\s]*$")
                .WithMessage("Street should only contain letters.");
            RuleFor(u => u.StreetNumber)
                .NotEmpty()
                .Matches(@"^[A-Za-z0-9\s]*$")
                .WithMessage("Street number should only contain letters and numbers.");
        }

        private bool BeAValidDate(DateTime date)
        {
            DateTime now = DateTime.Now;
            if (now.Year - date.Year > 120 | now.Year - date.Year < 16) return false;
            return true;
        }
    }
}