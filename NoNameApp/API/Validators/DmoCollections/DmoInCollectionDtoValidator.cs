using API.DTO.DmoCollections;
using FluentValidation;

namespace API.Validators.DmoCollections {
    // ReSharper disable UnusedMember.Global
    public class DmoInCollectionDtoValidator : AbstractValidator<DmoInCollectionDto> {
        public DmoInCollectionDtoValidator() {
            RuleFor(d => d.Id).NotEmpty().WithMessage("Dmo id is missing");
        }
    }
}
