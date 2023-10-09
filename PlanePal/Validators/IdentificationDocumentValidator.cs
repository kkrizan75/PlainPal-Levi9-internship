using FluentValidation;
using PlanePal.DTOs.User;

namespace PlanePal.Validators
{
    public class IdentificationDocumentValidator : AbstractValidator<DocumentDTO>
    {
        public IdentificationDocumentValidator()
        {
            RuleFor(d => d.DocumentNumber)
                .NotEmpty()
                .Matches(@"^[A-Za-z0-9]+$")
                .WithMessage("Document number must include only letters and numbers.");
            RuleFor(d => d.ExpirationDate)
                .NotNull()
                .Must(ValidExpirationDate)
                .WithMessage("Document cannot expire in less than 3 months.");
            RuleFor(d => d.DocumentType)
                .IsInEnum()
                .WithMessage("Document type does not exists.");
        }

        private bool ValidExpirationDate(DateTime date)
        {
            DateTime minValid = DateTime.Now.AddMonths(3);
            return date > minValid;
        }
    }
}