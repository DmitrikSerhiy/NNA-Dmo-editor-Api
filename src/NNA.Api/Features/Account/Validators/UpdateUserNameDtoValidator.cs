using FluentValidation;
using Microsoft.AspNetCore.Identity;
using NNA.Api.Helpers;
using NNA.Domain;
using NNA.Domain.DTOs.Account;

namespace NNA.Api.Features.Account.Validators;

public class UpdateUserNameDtoValidator : AbstractValidator<UpdateUserNameDto> {
    public UpdateUserNameDtoValidator() {
        RuleFor(u => u.Email)
            .NotEmpty()
            .WithMessage("Email is missing")
            .Matches(ValidatorHelpers.EmailRegex)
            .WithMessage("Invalid email address")
            .MaximumLength(ApplicationConstants.MaxUserEmailLength)
            .WithMessage($"Maximum email length is {ApplicationConstants.MaxUserEmailLength}");

        RuleFor(u => u.UserName)
            .NotEmpty()
            .WithMessage("UserName is missing")
            .MaximumLength(ApplicationConstants.MaxUserNameLength)
            .WithMessage($"Maximum user name length is {ApplicationConstants.MaxUserNameLength}")
            .Must(userName => userName.All(userNameSymbol =>
                (new UserOptions().AllowedUserNameCharacters += " ").Contains(userNameSymbol)))
            .WithMessage("User name may contain only letters numbers and -._@+ symbols");
    }
}