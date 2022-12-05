using FluentValidation;
using NNA.Domain.DTOs.Dmos;

namespace NNA.Api.Features.Dmos.Validators;

public sealed class AddConflictPairToDmoDtoValidator : AbstractValidator<AddConflictPairToDmoDto> {
    public AddConflictPairToDmoDtoValidator() {
        RuleFor(c => c.Protagonist)
            .NotEmpty()
            .WithMessage("Protagonist is missing");
        
        RuleFor(c => c.Antagonist)
            .NotEmpty()
            .WithMessage("Antagonist is missing");
        
        RuleFor(c => c.Antagonist.CharacterId)
            .NotEmpty()
            .WithMessage("Antagonist character id is missing");
        
        RuleFor(c => c.Protagonist.CharacterId)
            .NotEmpty()
            .WithMessage("Protagonist character id is missing");
        
        
        RuleFor(c => c.Antagonist.CharacterType)
            .IsInEnum()
            .WithMessage("Antagonist character type is invalid");
        
        RuleFor(c => c.Protagonist.CharacterType)
            .IsInEnum()
            .WithMessage("Protagonist character type is invalid");
    }
}
