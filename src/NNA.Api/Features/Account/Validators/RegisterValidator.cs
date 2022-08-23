using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Model;
using Model.DTOs.Account;
using Model.Entities;

namespace NNA.Api.Features.Account.Validators;
public class RegisterValidator : AbstractValidator<RegisterDto> {
    public RegisterValidator(PasswordValidator<NnaUser> passwordValidator) {
        RuleFor(u => u.UserName).NotEmpty().WithMessage("Username is missing");
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
            .NotEmpty().WithMessage("UserName is missing")
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
            .Must(password => password.Distinct().Count() > ApplicationConstants.MinPasswordLength / 2)
            .WithMessage($"Password must not contain {ApplicationConstants.MinPasswordLength / 2} repeating symbols")
            .Must(password => password.Any(passwordValidator.IsDigit))
            .WithMessage("Password must contain at least one number")
            .Must(password => password.Any(passwordValidator.IsLower))
            .WithMessage("Password must contain at least one symbol in lower case")
            .Must(password => password.Any(passwordValidator.IsUpper))
            .WithMessage("Password must contain at least one symbol in upper case");
    }
}