using FluentValidation;
using NNA.Api.Helpers;
using NNA.Domain;
using NNA.Domain.DTOs.Account;

namespace NNA.Api.Features.Account.Validators;

public class CheckEmailDtoValidator : AbstractValidator<CheckEmailDto> {
    public CheckEmailDtoValidator() {
        RuleFor(u => u.Email)
            .NotEmpty()
            .WithMessage("Email is missing")
            .Matches(ValidatorHelpers.EmailRegex)
            .WithMessage("Invalid email address")
            .MaximumLength(ApplicationConstants.MaxUserEmailLength)
            .WithMessage($"Maximum email length is {ApplicationConstants.MaxUserEmailLength}");
    }
}