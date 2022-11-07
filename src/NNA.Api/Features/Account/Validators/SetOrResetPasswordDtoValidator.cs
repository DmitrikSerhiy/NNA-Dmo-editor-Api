using FluentValidation;
using NNA.Api.Helpers;
using NNA.Domain;
using NNA.Domain.DTOs.Account;

namespace NNA.Api.Features.Account.Validators;

public sealed class SetOrResetPasswordDtoValidator : AbstractValidator<SetOrResetPasswordDto> {
    public SetOrResetPasswordDtoValidator() {
        RuleFor(u => u.Email)
            .NotEmpty()
            .WithMessage("Email is missing")
            .Matches(ValidatorHelpers.EmailRegex)
            .WithMessage("Invalid email address")
            .MaximumLength(ApplicationConstants.MaxUserEmailLength)
            .WithMessage($"Maximum email length is {ApplicationConstants.MaxUserEmailLength}");

        RuleFor(u => u.NewPassword)
            .NotEmpty()
            .WithMessage("Password is missing")
            .MinimumLength(ApplicationConstants.MinPasswordLength)
            .WithMessage($"Password must contain at least {ApplicationConstants.MinPasswordLength} symbols")
            .MaximumLength(ApplicationConstants.MaxPasswordLength)
            .WithMessage($"Maximum password length is {ApplicationConstants.MaxPasswordLength}")
            .Must(password => password.ToLower().Distinct().Count() > 5)
            .WithMessage($"Password must not contain 5 repeating symbols")
            .Must(password => password.Any(CharactersVerificator.IsDigit))
            .WithMessage("Password must contain at least one number")
            .Must(password => password.Any(CharactersVerificator.IsLower))
            .WithMessage("Password must contain at least one symbol in lower case")
            .Must(password => password.Any(CharactersVerificator.IsUpper))
            .WithMessage("Password must contain at least one symbol in upper case");

        RuleFor(u => u.Token)
            .NotEmpty()
            .WithMessage("Set or Reset token is missing");

        RuleFor(u => u.Reason)
            .NotEmpty()
            .WithMessage("Reason is missing")
            .IsInEnum()
            .WithMessage("Not valid reason");
    }
}