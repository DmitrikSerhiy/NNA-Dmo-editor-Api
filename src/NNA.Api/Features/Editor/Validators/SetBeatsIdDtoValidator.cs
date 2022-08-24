using FluentValidation;
using NNA.Domain.DTOs.Editor;

namespace NNA.Api.Features.Editor.Validators;
public class SetBeatsIdDtoValidator : AbstractValidator<SetBeatsIdDto> {
    public SetBeatsIdDtoValidator() {
        RuleFor(d => d.Id)
            .NotEmpty()
            .WithMessage("Dmo id is missing");
    }
}