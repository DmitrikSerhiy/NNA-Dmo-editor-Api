using FluentValidation;
using NNA.Domain.DTOs.Dmos;

namespace NNA.Api.Features.Dmos.Validators;

public sealed class SanitizeTempIdsInDmoDtoValidator : AbstractValidator<SanitizeTempIdsInDmoDto> {
    public SanitizeTempIdsInDmoDtoValidator() {
        RuleFor(dto => dto.DmoId)
            .NotEmpty()
            .WithMessage("Dmo id is missing");
    }
}
