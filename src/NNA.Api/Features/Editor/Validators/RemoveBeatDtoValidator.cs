using FluentValidation;
using NNA.Domain.DTOs.Editor;

namespace NNA.Api.Features.Editor.Validators;

public class RemoveBeatDtoValidator : AbstractValidator<RemoveBeatDto> {
    public RemoveBeatDtoValidator() {
        RuleFor(d => d.Id)
            .NotEmpty()
            .WithMessage("Beat TempId is missing");

        RuleFor(d => d.DmoId)
            .NotEmpty()
            .WithMessage("Dmo Id is missing");
    }
}