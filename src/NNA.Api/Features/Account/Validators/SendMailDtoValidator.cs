﻿using FluentValidation;
using NNA.Api.Helpers;
using NNA.Domain;
using NNA.Domain.DTOs.Account;

namespace NNA.Api.Features.Account.Validators;

public class SendMailDtoValidator : AbstractValidator<SendMailDto> {
    public SendMailDtoValidator() {
        RuleFor(u => u.Email)
            .NotEmpty()
            .WithMessage("Email is missing")
            .Matches(ValidatorHelpers.EmailRegex)
            .WithMessage("Invalid email address")
            .MaximumLength(ApplicationConstants.MaxUserEmailLength)
            .WithMessage($"Maximum email length is {ApplicationConstants.MaxUserEmailLength}");

        RuleFor(u => u.Reason)
            .NotEmpty()
            .WithMessage("Reason is missing")
            .IsInEnum()
            .WithMessage("Not valid reason");
    }
}