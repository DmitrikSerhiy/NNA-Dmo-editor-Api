using FluentValidation;
using NNA.Domain.DTOs.CharactersInBeats;

namespace NNA.Api.Features.Editor.Validators;

public sealed class DetachCharacterToBeatDtoValidator: AbstractValidator<DetachCharacterToBeatDto> {
    public DetachCharacterToBeatDtoValidator() {
        RuleFor(dto => dto.Id)
            .NotEmpty()
            .WithMessage("Id is missing");
        
        RuleFor(dto => dto.DmoId)
            .NotEmpty()
            .WithMessage("Dmo Id is missing");
        
        RuleFor(dto => dto.BeatId)
            .NotEmpty()
            .WithMessage("Beat Id is missing");
    }
}
