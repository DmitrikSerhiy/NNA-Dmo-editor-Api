using FluentValidation;
using Model;
using Model.DTOs.Account;

namespace API.Features.Account.Validators {
    
    public class AuthGoogleDtoValidator : AbstractValidator<AuthGoogleDto> {

        public AuthGoogleDtoValidator() {
            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("Email is missing")
                .Matches(@"^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$")
                .WithMessage("Invalid email address")
                .MaximumLength(ApplicationConstants.MaxUserEmailLength)
                .WithMessage($"Maximum email length is {ApplicationConstants.MaxUserEmailLength}");

            RuleFor(u => u.Name)
                .NotEmpty().WithMessage("UserName is missing")
                .MaximumLength(ApplicationConstants.MaxUserNameLength)
                .WithMessage($"Maximum user name length is {ApplicationConstants.MaxUserNameLength}");
            
            RuleFor(u => u.GoogleToken)
                .NotEmpty()
                .WithMessage("Google token is missing");
        }
    }
}