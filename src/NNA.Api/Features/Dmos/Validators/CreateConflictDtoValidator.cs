using FluentValidation;
using NNA.Domain.DTOs.Dmos;

namespace NNA.Api.Features.Dmos.Validators;

public sealed class CreateConflictDtoValidator: AbstractValidator<CreateConflictDto> {
    public CreateConflictDtoValidator() {
        RuleFor(c => c.PairOrder)
            .GreaterThan(-1)
            .WithMessage("Pair order cannot be negative value");
    }
}
