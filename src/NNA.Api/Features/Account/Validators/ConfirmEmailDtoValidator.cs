﻿using FluentValidation;
using NNA.Domain;
using NNA.Domain.DTOs.Account;

namespace NNA.Api.Features.Account.Validators;
public class ConfirmEmailDtoValidator : AbstractValidator<ConfirmEmailDto>{
    public ConfirmEmailDtoValidator() {
        RuleFor(u => u.Token)
            .NotEmpty()
            .WithMessage("Token is missing");
            
        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("Email is missing")
            .Matches(@"^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$")
            .WithMessage("Invalid email address")
            .MaximumLength(ApplicationConstants.MaxUserEmailLength)
            .WithMessage($"Maximum email length is {ApplicationConstants.MaxUserEmailLength}");
    }
}