using FluentValidation;
using PlanePal.DTOs.User;

namespace PlanePal.Validators
{
    public class UpdatePasswordValidator : AbstractValidator<UpdatePasswordDTO>
    {
        public UpdatePasswordValidator()
        {
            RuleFor(u => u.OldPassword)
                .NotEmpty();
            RuleFor(u => u.NewPassword)
                .NotEmpty()
                .Matches(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@#$%^&+=!])")
                .WithMessage("The password must include atleast 1 uppercase letter, 1 lowercase letter, " +
                "1 number and 1 special character")
                .MinimumLength(10);
        }
    }
}