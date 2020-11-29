using FluentValidation;
using Model.DTOs.DmoCollections;

namespace API.Features.DmoCollections.Validators {
    // ReSharper disable UnusedMember.Global
    public class DmoInCollectionDtoValidator : AbstractValidator<DmoInCollectionDto> {
        public DmoInCollectionDtoValidator() {
            RuleFor(d => d.Id).NotEmpty().WithMessage("Dmo id is missing");
        }
    }
}
