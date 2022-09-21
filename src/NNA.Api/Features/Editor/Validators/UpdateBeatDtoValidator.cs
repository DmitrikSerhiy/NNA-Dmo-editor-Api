using FluentValidation;
using NNA.Domain.DTOs.Editor;

namespace NNA.Api.Features.Editor.Validators;

public sealed class UpdateBeatDtoValidator : AbstractValidator<UpdateBeatDto> {
    public UpdateBeatDtoValidator() {
        RuleFor(d => d.BeatId)
            .NotEmpty()
            .WithMessage("Beat Id is missing");
    }
}