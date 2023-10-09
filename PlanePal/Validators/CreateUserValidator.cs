using FluentValidation;
using PlanePal.DTOs.User;

namespace PlanePal.Validators
{
    public class CreateUserValidator : AbstractValidator<CreateUserDTO>
    {
        public CreateUserValidator()
        {
            RuleFor(u => u.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("A valid email is required.");
            RuleFor(u => u.Password)
                .NotEmpty()
                .Matches(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@#$%^&+=!])")
                .WithMessage("Password must contain at least one upper case letter, one lower case letter, one number and one special character.")
                .MinimumLength(10)
                .WithMessage("Password must contain at least 10 characters");
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
            RuleFor(u => u.Address.City)
                .NotEmpty()
                .Matches(@"^[A-Za-z\s]*$")
                .WithMessage("City should only contain letters.");
            RuleFor(u => u.Address.Country)
                .NotEmpty()
                .Matches(@"^[A-Za-z\s]*$")
                .WithMessage("Country should only contain letters.");
            RuleFor(u => u.Address.Street)
                .NotEmpty()
                .Matches(@"^[A-Za-z\s]*$")
                .WithMessage("Street should only contain letters.");
            RuleFor(u => u.Address.StreetNumber)
                .NotEmpty()
                .Matches(@"^[A-Za-z0-9\s]*$")
                .WithMessage("Street number should only contain letters and numbers.");

            When(u => u.IdentificationDocument != null, () =>
            {
                RuleFor(u => u.IdentificationDocument.DocumentNumber)
                .Matches(@"^[A-Za-z0-9]+$")
                .WithMessage("Document number must include only letters and numbers.");
                RuleFor(d => d.IdentificationDocument.ExpirationDate)
                    .Must(ValidExpirationDate)
                    .WithMessage("Document cannot expire in less than 3 months.");
                RuleFor(d => d.IdentificationDocument.DocumentType)
                   .IsInEnum()
                   .WithMessage("Document type does not exist.");
            });
        }

        private bool BeAValidDate(DateTime date)
        {
            DateTime now = DateTime.Now;
            if (now.Year - date.Year > 120 | now.Year - date.Year < 16) return false;
            return true;
        }

        private bool ValidExpirationDate(DateTime date)
        {
            DateTime minValid = DateTime.Now.AddMonths(3);
            return date > minValid;
        }
    }
}