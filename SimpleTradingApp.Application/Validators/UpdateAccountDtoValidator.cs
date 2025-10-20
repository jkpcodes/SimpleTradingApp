using FluentValidation;
using SimpleTradingApp.Application.DTOs;

namespace SimpleTradingApp.Application.Validators;

public class UpdateAccountDtoValidator : AbstractValidator<UpdateAccountDto>
{
    public UpdateAccountDtoValidator()
    {
        RuleFor(x => x.ID)
            .NotEmpty().WithMessage("Account ID is required.");
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.");
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.");
    }
}
