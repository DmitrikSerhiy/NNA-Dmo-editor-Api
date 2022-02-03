using FluentValidation;
using Model;
using Model.DTOs.Account;

namespace API.Features.Account.Validators {
    public class CheckNameDtoValidator : AbstractValidator<CheckNameDto> {
        
        public CheckNameDtoValidator() {
            RuleFor(u => u.Name)
                .NotEmpty()
                .WithMessage("UserName is missing")
                .MaximumLength(ApplicationConstants.MaxUserNameLength)
                .WithMessage($"Maximum user name length is {ApplicationConstants.MaxUserNameLength}");
        }
    }
}