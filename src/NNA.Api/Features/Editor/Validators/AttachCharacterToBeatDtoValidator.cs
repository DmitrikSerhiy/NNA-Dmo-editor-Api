using FluentValidation;
using NNA.Domain.DTOs.CharactersInBeats;

namespace NNA.Api.Features.Editor.Validators;

public sealed class AttachCharacterToBeatDtoValidator : AbstractValidator<AttachCharacterToBeatDto> {
    public AttachCharacterToBeatDtoValidator() {
        RuleFor(dto => dto.DmoId)
            .NotEmpty()
            .WithMessage("Dmo Id is missing");
        
        RuleFor(dto => dto.BeatId)
            .NotEmpty()
            .WithMessage("Beat Id is missing");
        
        RuleFor(dto => dto.CharacterId)
            .NotEmpty()
            .WithMessage("Character Id is missing");
    }
}
