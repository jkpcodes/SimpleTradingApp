using FluentValidation;
using SimpleTradingApp.Application.DTOs;

namespace SimpleTradingApp.Application.Validators;

public class AddAccountDtoValidator : AbstractValidator<AddAccountDto>
{
    public AddAccountDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.");
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.");
    }
}
