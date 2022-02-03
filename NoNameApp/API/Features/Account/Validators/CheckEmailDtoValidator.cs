using FluentValidation;
using Model;
using Model.DTOs.Account;

namespace API.Features.Account.Validators {
    public class CheckEmailDtoValidator: AbstractValidator<CheckEmailDto> {

        public CheckEmailDtoValidator() {
            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("Email is missing")
                .Matches(@"^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$")
                .WithMessage("Invalid email address")
                .MaximumLength(ApplicationConstants.MaxUserEmailLength)
                .WithMessage($"Maximum email length is {ApplicationConstants.MaxUserEmailLength}");
        }
    }
}