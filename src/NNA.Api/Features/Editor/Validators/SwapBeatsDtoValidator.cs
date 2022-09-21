using FluentValidation;
using NNA.Domain.DTOs.Editor;

namespace NNA.Api.Features.Editor.Validators;

public sealed class SwapBeatsDtoValidator : AbstractValidator<SwapBeatsDto> {
    public SwapBeatsDtoValidator() {
        RuleFor(beat => beat.dmoId)
            .NotEmpty()
            .WithMessage("Dmo id is missing");
        RuleFor(beat => beat.beatToMove.id)
            .NotEmpty()
            .WithMessage("Beat id to move is missing");
        RuleFor(beat => beat.beatToReplace.id)
            .NotEmpty()
            .WithMessage("Beat id to replace is missing");
    }
}
