using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Model;
using Model.DTOs.Account;

namespace NNA.Api.Features.Account.Validators;
public class CheckNameDtoValidator : AbstractValidator<CheckNameDto> {
    public CheckNameDtoValidator() {
        RuleFor(u => u.Name)
            .NotEmpty()
            .WithMessage("UserName is missing")
            .MaximumLength(ApplicationConstants.MaxUserNameLength)
            .WithMessage($"Maximum user name length is {ApplicationConstants.MaxUserNameLength}")
            .Must(userName => userName.All(userNameSymbol => (new UserOptions().AllowedUserNameCharacters += " ").Contains(userNameSymbol)))
            .WithMessage("User name may contain only letters, numbers and -._@+ symbols");
    }
}