using FluentValidation;
using NNA.Domain.DTOs.Editor;

namespace NNA.Api.Features.Editor.Validators;

public sealed class MoveBeatDtoValidator : AbstractValidator<MoveBeatDto> {
    public MoveBeatDtoValidator() {
        RuleFor(d => d.dmoId)
            .NotEmpty()
            .WithMessage("Dmo id is missing");
        
        RuleFor(d => d.id)
            .NotEmpty()
            .WithMessage("Beat id is missing");
    }
}
