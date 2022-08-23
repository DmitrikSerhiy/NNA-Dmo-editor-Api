using FluentValidation;
using Model.DTOs.Editor;

namespace NNA.Api.Features.Editor.Validators;
public class LoadShortDmoDtoValidator : AbstractValidator<LoadShortDmoDto> {
    public LoadShortDmoDtoValidator() {
        RuleFor(d => d.Id)
            .NotEmpty()
            .WithMessage("Dmo id is missing");
    }
}
