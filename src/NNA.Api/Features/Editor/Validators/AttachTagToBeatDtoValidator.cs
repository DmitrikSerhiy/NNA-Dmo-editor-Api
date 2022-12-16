using FluentValidation;
using NNA.Domain.DTOs.TagsInBeats;

namespace NNA.Api.Features.Editor.Validators;

public sealed class AttachTagToBeatDtoValidator : AbstractValidator<AttachTagToBeatDto> {
    public AttachTagToBeatDtoValidator()
    {
        RuleFor(dto => dto.Id)
            .NotEmpty()
            .WithMessage("Id is missing");
        
        RuleFor(dto => dto.DmoId)
            .NotEmpty()
            .WithMessage("Dmo Id is missing");
        
        RuleFor(dto => dto.BeatId)
            .NotEmpty()
            .WithMessage("Beat Id is missing");
        
        RuleFor(dto => dto.TagId)
            .NotEmpty()
            .WithMessage("Tag Id is missing");
    }
}