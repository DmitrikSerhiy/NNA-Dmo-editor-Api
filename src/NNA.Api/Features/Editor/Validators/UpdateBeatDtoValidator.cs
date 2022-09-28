using FluentValidation;
using NNA.Domain.DTOs.Editor;

namespace NNA.Api.Features.Editor.Validators;

public sealed class UpdateBeatDtoValidator : AbstractValidator<UpdateBeatDto> {
    public UpdateBeatDtoValidator() {
        RuleFor(d => d.BeatId)
            .NotEmpty()
            .WithMessage("Beat Id is missing");

        RuleFor(d => d.Time.Minutes)
            .InclusiveBetween(0, 59)
            .WithMessage("Minutes should be in between of 0 and 59");
        
        RuleFor(d => d.Time.Seconds)
            .InclusiveBetween(0, 59)
            .WithMessage("Seconds should be in between of 0 and 59");
    }
}