using FluentValidation;
using Model.Account;

namespace API.Validators.Account
{
    // ReSharper disable once UnusedMember.Global
    public class LoginValidator : AbstractValidator<LoginModel>
    {
        public LoginValidator() {
            
            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("Email is missing")
                .EmailAddress().WithMessage("Invalid email address");
            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("Password is missing")
                .MinimumLength(8).WithMessage("Password must contain at least 8 symbols");
        }
    }
}
