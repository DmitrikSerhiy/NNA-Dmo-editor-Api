using FluentValidation;
using NNA.Domain.DTOs.Dmos;

namespace NNA.Api.Features.Dmos.Validators;

public sealed class UpdateDmoConflictDtoValidator: AbstractValidator<UpdateDmoConflictDto> {
    public UpdateDmoConflictDtoValidator() {
        RuleFor(c => c.CharacterId)
            .NotEmpty()
            .WithMessage("Character id is missing");
    }
}
