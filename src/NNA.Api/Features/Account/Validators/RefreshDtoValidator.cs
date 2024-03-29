﻿using FluentValidation;
using NNA.Domain.DTOs.Account;

namespace NNA.Api.Features.Account.Validators;

public sealed class RefreshDtoValidator : AbstractValidator<RefreshDto> {
    public RefreshDtoValidator() {
        RuleFor(u => u.AccessToken)
            .NotEmpty()
            .WithMessage("AccessToken is missing");

        RuleFor(u => u.RefreshToken)
            .NotEmpty()
            .WithMessage("RefreshToken is missing");
    }
}