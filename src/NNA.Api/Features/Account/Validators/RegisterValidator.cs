using FluentValidation;
using Microsoft.AspNetCore.Identity;
using NNA.Api.Helpers;
using NNA.Domain;
using NNA.Domain.DTOs.Account;

namespace NNA.Api.Features.Account.Validators;

public sealed class RegisterValidator : AbstractValidator<RegisterDto> {
    public RegisterValidator() {
        RuleFor(u => u.Email)
            .NotEmpty()
            .WithMessage("Email is missing")
            .Matches(ValidatorHelpers.EmailRegex)
            .WithMessage("Invalid email address")
            .MaximumLength(ApplicationConstants.MaxUserEmailLength)
            .WithMessage($"Maximum email length is {ApplicationConstants.MaxUserEmailLength}");

        RuleFor(u => u.UserName)
            .NotEmpty()
            .WithMessage("Nickname is missing")
            .MaximumLength(ApplicationConstants.MaxUserNameLength)
            .WithMessage($"Maximum user nickname length is {ApplicationConstants.MaxUserNameLength}")
            .Must(userName => userName.All(userNameSymbol =>
                (new UserOptions().AllowedUserNameCharacters += " ").Contains(userNameSymbol)))
            .WithMessage("Nickname may contain only letters, numbers and -._@+ symbols");

        RuleFor(u => u.Password)
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
    }
}