using FluentValidation;
using NNA.Domain.DTOs.Editor;

namespace NNA.Api.Features.Editor.Validators;
public class UpdateBeatDtoValidator : AbstractValidator<UpdateBeatDto> {
    public UpdateBeatDtoValidator() {
        RuleFor(d => d.BeatId)
            .NotEmpty()
            .WithMessage("Beat Id is missing");
    }
}