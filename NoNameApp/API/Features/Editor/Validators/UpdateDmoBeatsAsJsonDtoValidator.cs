using FluentValidation;
using Model.DTOs.Editor;

namespace API.Features.Editor.Validators
{
    public class UpdateDmoBeatsAsJsonDtoValidator : AbstractValidator<UpdateDmoBeatsAsJsonDto> {
        public UpdateDmoBeatsAsJsonDtoValidator() {
            RuleFor(d => d.DmoId)
                .NotEmpty()
                .WithMessage("Dmo id is missing");
        }
    }
}
