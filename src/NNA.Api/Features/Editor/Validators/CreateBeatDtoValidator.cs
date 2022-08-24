using FluentValidation;
using NNA.Domain.DTOs.Editor;

namespace NNA.Api.Features.Editor.Validators;
public class CreateBeatDtoValidator : AbstractValidator<CreateBeatDto> {
    public CreateBeatDtoValidator() {
        RuleFor(d => d.TempId)
            .NotEmpty()
            .WithMessage("Beat TempId is missing");

        RuleFor(d => d.DmoId)
            .NotEmpty()
            .WithMessage("Dmo Id is missing");
    }
}