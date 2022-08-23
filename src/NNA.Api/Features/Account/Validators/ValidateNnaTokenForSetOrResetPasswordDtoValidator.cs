using FluentValidation;
using Model;
using Model.DTOs.Account;

namespace NNA.Api.Features.Account.Validators;
public class ValidateNnaTokenForSetOrResetPasswordDtoValidator: AbstractValidator<ValidateNnaTokenForSetOrResetPasswordDto> {
    public ValidateNnaTokenForSetOrResetPasswordDtoValidator() {
        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("Email is missing")
            .Matches(@"^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$")
            .WithMessage("Invalid email address")
            .MaximumLength(ApplicationConstants.MaxUserEmailLength)
            .WithMessage($"Maximum email length is {ApplicationConstants.MaxUserEmailLength}");
            
        RuleFor(u => u.Token)
            .NotEmpty()
            .WithMessage("Token is missing");
            
        RuleFor(u => u.Reason)
            .IsInEnum()
            .WithMessage("Not valid reason");
    }
}