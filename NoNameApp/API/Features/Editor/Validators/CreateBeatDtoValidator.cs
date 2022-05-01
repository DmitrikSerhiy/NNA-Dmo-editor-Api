using FluentValidation;
using Model.DTOs.Editor;

namespace API.Features.Editor.Validators {
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
}