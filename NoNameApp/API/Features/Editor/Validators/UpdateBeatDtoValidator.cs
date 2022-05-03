using FluentValidation;
using Model.DTOs.Editor;

namespace API.Features.Editor.Validators {
    public class UpdateBeatDtoValidator : AbstractValidator<UpdateBeatDto> {
        public UpdateBeatDtoValidator() {
            RuleFor(d => d.BeatId)
                .NotEmpty()
                .WithMessage("Beat Id is missing");
        }
    }
}