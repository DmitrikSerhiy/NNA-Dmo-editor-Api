using FluentValidation;
using Model.DTOs.DmoCollections;

namespace API.Features.DmoCollections.Validators {
    // ReSharper disable UnusedMember.Global
    public class AddDmoToCollectionDtoValidator : AbstractValidator<AddDmoToCollectionDto> {
        public AddDmoToCollectionDtoValidator() {
            RuleFor(d => d.CollectionId).NotEmpty().WithMessage("Collection id is missing");
            RuleFor(d => d.Dmos).NotEmpty().WithMessage("No dmo to add to collection");
        }
    }
}
