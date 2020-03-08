using API.DTO.DmoCollections;
using FluentValidation;

namespace API.Validators.DmoCollections {
    // ReSharper disable UnusedMember.Global
    public class DeleteCollectionDtoValidator : AbstractValidator<DeleteCollectionDto> {
        public DeleteCollectionDtoValidator() {
            RuleFor(d => d.CollectionId).NotEmpty().WithMessage("Collection id is missing");
        }
    }
}
