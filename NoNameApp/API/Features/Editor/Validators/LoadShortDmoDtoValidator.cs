using FluentValidation;
using Model.DTOs.Editor;

namespace API.Features.Editor.Validators {
    public class LoadShortDmoDtoValidator : AbstractValidator<LoadShortDmoDto> {
        public LoadShortDmoDtoValidator() {
            RuleFor(d => d.Id)
                .NotEmpty()
                .WithMessage("Dmo id is missing");
        }
    }
}
