using FluentValidation;
using Model.DTOs.DmoCollections;

namespace NNA.Api.Features.DmoCollections.Validators;
public class RemoveDmoFromCollectionDtoValidator : AbstractValidator<RemoveDmoFromCollectionDto> {
    public RemoveDmoFromCollectionDtoValidator() {
        RuleFor(d => d.CollectionId).NotEmpty().WithMessage("Collection id is missing");
        RuleFor(d => d.DmoId).NotEmpty().WithMessage("Dmo id is missing");
    }
}