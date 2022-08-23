using FluentValidation;
using Model.DTOs.DmoCollections;

namespace NNA.Api.Features.DmoCollections.Validators;
public class CollectionNameDtoValidator : AbstractValidator<CollectionNameDto> {
    public CollectionNameDtoValidator() {
        RuleFor(d => d.CollectionId).NotEmpty().WithMessage("Collection id is missing");
    }
}