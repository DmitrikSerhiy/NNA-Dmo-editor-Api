using FluentValidation;
using Model.DTOs.Editor;

namespace API.Features.Editor.Validators {
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
}