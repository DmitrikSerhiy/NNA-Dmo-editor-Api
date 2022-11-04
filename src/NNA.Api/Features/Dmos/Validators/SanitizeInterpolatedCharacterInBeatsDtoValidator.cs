using FluentValidation;
using NNA.Domain.DTOs.Dmos;

namespace NNA.Api.Features.Dmos.Validators;

public sealed class SanitizeInterpolatedCharacterInBeatsDtoValidator : AbstractValidator<SanitizeInterpolatedCharacterInBeatsDto> {
    public SanitizeInterpolatedCharacterInBeatsDtoValidator() {
        RuleFor(d => d.CharacterIds)
            .NotEmpty()
            .WithMessage("Characters in beat id is missing");
        
        RuleForEach(x => x.CharacterIds)
            .NotEmpty()
            .WithMessage("Collection contains empty or not valid id");


    }
}
