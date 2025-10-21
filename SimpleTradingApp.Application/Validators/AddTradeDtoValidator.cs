using FluentValidation;
using SimpleTradingApp.Application.DTOs;

namespace SimpleTradingApp.Application.Validators;

public class AddTradeDtoValidator : AbstractValidator<AddTradeDto>
{
    public AddTradeDtoValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("Account ID is required.");
        RuleFor(x => x.SecurityCode)
            .NotEmpty().WithMessage("Security code is required.")
            .Length(3).WithMessage("The Security Code must be exactly 3 characters long.");
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");
        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Type must be a valid trade type.");
    }
}
