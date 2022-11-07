using FluentValidation;
using NNA.Domain.DTOs.Account;

namespace NNA.Api.Features.Account.Validators;

public sealed class TokensDtoValidator : AbstractValidator<TokensDto> {
    public TokensDtoValidator() {
        RuleFor(u => u.RefreshToken)
            .NotEmpty()
            .WithMessage("RefreshToken is missing");

        RuleFor(u => u.AccessToken)
            .NotEmpty()
            .WithMessage("AccessToken is missing");

        RuleFor(u => u.AccessTokenKeyId)
            .NotEmpty()
            .WithMessage("AccessTokenKeyId is missing");

        RuleFor(u => u.RefreshTokenKeyId)
            .NotEmpty()
            .WithMessage("RefreshTokenKeyId is missing");
    }
}