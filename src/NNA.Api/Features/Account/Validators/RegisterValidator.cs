using FluentValidation;
using Microsoft.AspNetCore.Identity;
using NNA.Api.Helpers;
using NNA.Domain;
using NNA.Domain.DTOs.Account;

namespace NNA.Api.Features.Account.Validators;
public class RegisterValidator : AbstractValidator<RegisterDto> {
    public RegisterValidator() {
        // regex html5 standard. Took from https://html.spec.whatwg.org/multipage/input.html#valid-e-mail-address
        // it fit angular built in validation
        RuleFor(u => u.Email)
            .NotEmpty()
            .WithMessage("Email is missing")
            .Matches(@"^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$")
            .WithMessage("Invalid email address")
            .MaximumLength(ApplicationConstants.MaxUserEmailLength)
            .WithMessage($"Maximum email length is {ApplicationConstants.MaxUserEmailLength}");

        RuleFor(u => u.UserName)
            .NotEmpty()
            .WithMessage("UserName is missing")
            .MaximumLength(ApplicationConstants.MaxUserNameLength)
            .WithMessage($"Maximum user name length is {ApplicationConstants.MaxUserNameLength}")
            .Must(userName => userName.All(userNameSymbol => (new UserOptions().AllowedUserNameCharacters += " ").Contains(userNameSymbol)))
            .WithMessage("User name may contain only letters, numbers and -._@+ symbols");

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