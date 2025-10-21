using FluentValidation;
using SimpleTradingApp.Application.DTOs;

namespace SimpleTradingApp.Application.Validators;

public class UpdateTradeStatusDtoValidator : AbstractValidator<UpdateTradeStatusDto>
{
    public UpdateTradeStatusDtoValidator()
    {
        RuleFor(x => x.ID)
            .NotEmpty().WithMessage("Trade ID is required.");
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status must be a valid trade status.");
    }
}
