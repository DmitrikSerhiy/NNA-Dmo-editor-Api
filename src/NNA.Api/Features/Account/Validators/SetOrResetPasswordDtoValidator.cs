using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Model;
using Model.DTOs.Account;
using Model.Entities;

namespace NNA.Api.Features.Account.Validators;
public class SetOrResetPasswordDtoValidator: AbstractValidator<SetOrResetPasswordDto> {
    public SetOrResetPasswordDtoValidator(PasswordValidator<NnaUser> passwordValidator) {
            
        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("Email is missing")
            .Matches(@"^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$")
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
            .Must(password => password.Distinct().Count() > ApplicationConstants.MinPasswordLength / 2)
            .WithMessage($"Password must not contain {ApplicationConstants.MinPasswordLength / 2} repeating symbols")
            .Must(password => password.Any(passwordValidator.IsDigit))
            .WithMessage("Password must contain at least one number")
            .Must(password => password.Any(passwordValidator.IsLower))
            .WithMessage("Password must contain at least one symbol in lower case")
            .Must(password => password.Any(passwordValidator.IsUpper))
            .WithMessage("Password must contain at least one symbol in upper case");

        RuleFor(u => u.Token)
            .NotEmpty()
            .WithMessage("Set or Reset token is missing");
            
        RuleFor(u => u.Reason)
            .IsInEnum()
            .WithMessage("Not valid reason");
    }
}