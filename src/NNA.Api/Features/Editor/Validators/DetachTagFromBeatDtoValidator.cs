using FluentValidation;
using NNA.Domain.DTOs.TagsInBeats;

namespace NNA.Api.Features.Editor.Validators;

public sealed class DetachTagFromBeatDtoValidator : AbstractValidator<DetachTagFromBeatDto> {
    public DetachTagFromBeatDtoValidator() {
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