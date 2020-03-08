using API.DTO.DmoCollections;
using FluentValidation;

namespace API.Validators.DmoCollections {
    // ReSharper disable UnusedMember.Global
    public class RemoveDmoFromCollectionDtoValidator : AbstractValidator<RemoveDmoFromCollectionDto> {
        public RemoveDmoFromCollectionDtoValidator() {
            RuleFor(d => d.CollectionId).NotEmpty().WithMessage("Collection id is missing");
            RuleFor(d => d.DmoId).NotEmpty().WithMessage("Dmo id is missing");
        }
    }
}
