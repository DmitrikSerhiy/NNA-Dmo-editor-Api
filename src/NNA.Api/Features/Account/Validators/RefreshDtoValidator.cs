using FluentValidation;
using Model.DTOs.Account;

namespace NNA.Api.Features.Account.Validators;
public class RefreshDtoValidator: AbstractValidator<RefreshDto> {
    public RefreshDtoValidator() {
        RuleFor(u => u.AccessToken)
            .NotEmpty()
            .WithMessage("AccessToken is missing");
            
        RuleFor(u => u.RefreshToken)
            .NotEmpty()
            .WithMessage("RefreshToken is missing");
    }
}