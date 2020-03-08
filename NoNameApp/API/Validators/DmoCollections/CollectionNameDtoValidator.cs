using API.DTO.DmoCollections;
using FluentValidation;

namespace API.Validators.DmoCollections {
    // ReSharper disable UnusedMember.Global
    public class CollectionNameDtoValidator : AbstractValidator<CollectionNameDto> {
        public CollectionNameDtoValidator() {
            RuleFor(d => d.CollectionId).NotEmpty().WithMessage("Collection id is missing");
        }
    }
}
