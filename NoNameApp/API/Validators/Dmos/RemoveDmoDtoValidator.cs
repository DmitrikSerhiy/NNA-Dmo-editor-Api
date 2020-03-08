using API.DTO.Dmos;
using FluentValidation;

namespace API.Validators.Dmos {
    // ReSharper disable UnusedMember.Global
    public class RemoveDmoDtoValidator : AbstractValidator<RemoveDmoDto> {
        public RemoveDmoDtoValidator() {
            RuleFor(d => d.DmoId).NotEmpty().WithMessage("Dmo id is missing");
        }
    }
}
