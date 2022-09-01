using FluentValidation;
using NNA.Domain.DTOs.Editor;

namespace NNA.Api.Features.Editor.Validators;

public class UpdateDmoBeatsAsJsonDtoValidator : AbstractValidator<UpdateDmoBeatsAsJsonDto> {
    public UpdateDmoBeatsAsJsonDtoValidator() {
        RuleFor(d => d.DmoId)
            .NotEmpty()
            .WithMessage("Dmo id is missing");
    }
}