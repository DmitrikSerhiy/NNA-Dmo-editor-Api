﻿
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Model;
using Model.DTOs.Account;
using Model.Entities;

namespace API.Features.Account.Validators {
    
    public class ChangePasswordDtoValidator: AbstractValidator<ChangePasswordDto> {

        public ChangePasswordDtoValidator(PasswordValidator<NnaUser> passwordValidator) {
            RuleFor(u => u.CurrentPassword)
                .NotEmpty()
                .WithMessage("Password is missing")
                .MinimumLength(ApplicationConstants.MinPasswordLength)
                .WithMessage($"Password must contain at least {ApplicationConstants.MinPasswordLength} symbols")
                .MaximumLength(ApplicationConstants.MaxPasswordLength)
                .WithMessage($"Maximum password length is {ApplicationConstants.MaxPasswordLength}")
                .Must(password => password.Distinct().Count() > ApplicationConstants.MinPasswordLength / 2)
                .WithMessage($"Passwords must use at least {ApplicationConstants.MinPasswordLength / 2} different symbols")
                .Must(password => password.Any(passwordValidator.IsDigit))
                .WithMessage("Password must contain at least one number")
                .Must(password => password.Any(passwordValidator.IsLower))
                .WithMessage("Password must contain at least one symbol in lower case")
                .Must(password => password.Any(passwordValidator.IsUpper))
                .WithMessage("Password must contain at least one symbol in upper case");
            
            RuleFor(u => u.NewPassword)
                .NotEmpty()
                .WithMessage("Password is missing")
                .MinimumLength(ApplicationConstants.MinPasswordLength)
                .WithMessage($"Password must contain at least {ApplicationConstants.MinPasswordLength} symbols")
                .MaximumLength(ApplicationConstants.MaxPasswordLength)
                .WithMessage($"Maximum password length is {ApplicationConstants.MaxPasswordLength}")
                .Must(password => password.Distinct().Count() > ApplicationConstants.MinPasswordLength / 2)
                .WithMessage($"Passwords must use at least {ApplicationConstants.MinPasswordLength / 2} different symbols")
                .Must(password => password.Any(passwordValidator.IsDigit))
                .WithMessage("Password must contain at least one number")
                .Must(password => password.Any(passwordValidator.IsLower))
                .WithMessage("Password must contain at least one symbol in lower case")
                .Must(password => password.Any(passwordValidator.IsUpper))
                .WithMessage("Password must contain at least one symbol in upper case");
        }
    }
}