using FluentValidation;
using NNA.Domain.DTOs.Dmos;

namespace NNA.Api.Features.Dmos.Validators;
public class RemoveDmoDtoValidator : AbstractValidator<RemoveDmoDto> {
    public RemoveDmoDtoValidator() {
        RuleFor(d => d.DmoId).NotEmpty().WithMessage("Dmo id is missing");
    }
}
