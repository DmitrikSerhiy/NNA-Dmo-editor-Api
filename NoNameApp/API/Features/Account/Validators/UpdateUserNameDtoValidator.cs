using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Model;
using Model.DTOs.Account;

namespace API.Features.Account.Validators {
    public class UpdateUserNameDtoValidator: AbstractValidator<UpdateUserNameDto> {
        public UpdateUserNameDtoValidator() {
            
            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("Email is missing")
                .Matches(@"^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$")
                .WithMessage("Invalid email address")
                .MaximumLength(ApplicationConstants.MaxUserEmailLength)
                .WithMessage($"Maximum email length is {ApplicationConstants.MaxUserEmailLength}");
            
            RuleFor(u => u.UserName)
                .NotEmpty().WithMessage("UserName is missing")
                .MaximumLength(ApplicationConstants.MaxUserNameLength)
                .WithMessage($"Maximum user name length is {ApplicationConstants.MaxUserNameLength}")
                .Must(userName => userName.All(userNameSymbol => (new UserOptions().AllowedUserNameCharacters += " ").Contains(userNameSymbol)))
                .WithMessage("User name may contain only letters numbers and -._@+ symbols");
        }
    }
}