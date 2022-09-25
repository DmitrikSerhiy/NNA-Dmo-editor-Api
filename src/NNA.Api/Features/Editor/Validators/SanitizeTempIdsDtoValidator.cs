using FluentValidation;
using NNA.Domain.DTOs.Editor;

namespace NNA.Api.Features.Editor.Validators;

public sealed class SanitizeTempIdsDtoValidator : AbstractValidator<SanitizeTempIdsDto> {
    public SanitizeTempIdsDtoValidator() {
        RuleFor(beat => beat.dmoId)
            .NotEmpty()
            .WithMessage("Dmo Id is missing");
    }
}
