using FluentValidation;
using Model.Account;

namespace API.Validators.Account {
    // ReSharper disable once UnusedMember.Global
    public class RegisterValidator : AbstractValidator<RegisterModel> {
        public RegisterValidator() {
            RuleFor(u => u.UserName).NotEmpty().WithMessage("Username is missing");
            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("Email is missing")
                .EmailAddress().WithMessage("Invalid email address");
            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("Password is missing")
                .MinimumLength(8).WithMessage("Password must contain at least 8 symbols");
        }
    }
}
