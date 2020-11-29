using FluentValidation;
using Model.DTOs.Dmos;

namespace API.Features.Dmos.Validators {
    // ReSharper disable UnusedMember.Global
    public class RemoveDmoDtoValidator : AbstractValidator<RemoveDmoDto> {
        public RemoveDmoDtoValidator() {
            RuleFor(d => d.DmoId).NotEmpty().WithMessage("Dmo id is missing");
        }
    }
}
