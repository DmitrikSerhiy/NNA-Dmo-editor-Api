using FluentValidation;
using Model.DTOs.DmoCollections;

namespace API.Features.DmoCollections.Validators {
    // ReSharper disable UnusedMember.Global
    public class DeleteCollectionDtoValidator : AbstractValidator<DeleteCollectionDto> {
        public DeleteCollectionDtoValidator() {
            RuleFor(d => d.CollectionId).NotEmpty().WithMessage("Collection id is missing");
        }
    }
}
