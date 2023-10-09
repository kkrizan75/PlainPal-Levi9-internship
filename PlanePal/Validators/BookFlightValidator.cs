using FluentValidation;
using PlanePal.DTOs.BookedFlight;

namespace PlanePal.Validators
{
    public class BookFlightValidator : AbstractValidator<BookFlightDTO>
    {
        public BookFlightValidator()
        {
            RuleFor(bf => bf.TicketQuantity)
                .Must(t => t > 0 && t <= 5)
                .WithMessage("Ticket quantity should be greater than 0 but lesser than 5.");
        }
    }
}