using FluentValidation;
using Model;
using Model.Account;

namespace API.Validators.Account {
    // ReSharper disable once UnusedMember.Global
    public class RegisterValidator : AbstractValidator<RegisterModel> {
        public RegisterValidator() {
            RuleFor(u => u.UserName).NotEmpty().WithMessage("Username is missing");
            //regex html5 standard. Took from https://html.spec.whatwg.org/multipage/input.html#valid-e-mail-address
            //so it fit angular built in validation
            RuleFor(u => u.Email)
                .NotEmpty()
                    .WithMessage("Email is missing")
                .Matches(@"^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$")
                    .WithMessage("Invalid email address")
                .MaximumLength(ApplicationConstants.MaxUserEmailLength)
                    .WithMessage($"Maximum email length is {ApplicationConstants.MaxUserEmailLength}");

            RuleFor(u => u.Password)
                .NotEmpty()
                    .WithMessage("Password is missing")
                .MinimumLength(ApplicationConstants.MinPasswordLength)
                    .WithMessage($"Password must contain at least {ApplicationConstants.MinPasswordLength} symbols")
                .MaximumLength(ApplicationConstants.MaxPasswordLength)
                    .WithMessage($"Maximum password length is {ApplicationConstants.MaxPasswordLength}");
        }
    }
}
