using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Model;
using Model.DTOs.Account;

namespace API.Features.Account.Validators {
    public class UpdateUserNameDtoValidator: AbstractValidator<UpdateUserNameDto> {
        
        public UpdateUserNameDtoValidator() {
            RuleFor(u => u.UserName)
                .NotEmpty().WithMessage("UserName is missing")
                .MaximumLength(ApplicationConstants.MaxUserNameLength)
                .WithMessage($"Maximum user name length is {ApplicationConstants.MaxUserNameLength}")
                .Must(userName => userName.All(userNameSymbol => (new UserOptions().AllowedUserNameCharacters += " ").Contains(userNameSymbol)))
                .WithMessage("User name may contain only letters numbers and -._@+ symbols");
        }
    }
}