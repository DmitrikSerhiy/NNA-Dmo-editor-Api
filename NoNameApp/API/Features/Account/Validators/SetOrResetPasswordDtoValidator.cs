using FluentValidation;
using Model;
using Model.DTOs.Account;

namespace API.Features.Account.Validators {
    public class SetOrResetPasswordDtoValidator: AbstractValidator<SetOrResetPasswordDto> {

        public SetOrResetPasswordDtoValidator() {
            
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
                .WithMessage($"Maximum password length is {ApplicationConstants.MaxPasswordLength}");

            RuleFor(u => u.Token)
                .NotEmpty()
                .WithMessage("Set or Reset token is missing");
            
            RuleFor(u => u.Reason)
                .IsInEnum()
                .WithMessage("Not valid reason");
        }
    }
}