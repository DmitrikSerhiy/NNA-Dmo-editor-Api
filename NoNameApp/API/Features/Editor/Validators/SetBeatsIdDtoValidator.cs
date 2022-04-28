using FluentValidation;
using Model.DTOs.Editor;

namespace API.Features.Editor.Validators {
    public class SetBeatsIdDtoValidator: AbstractValidator<SetBeatsIdDto> {
        public SetBeatsIdDtoValidator() {
            RuleFor(d => d.Id)
                .NotEmpty()
                .WithMessage("Dmo id is missing");
        }
    }
}