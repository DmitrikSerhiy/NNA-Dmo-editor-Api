using FluentValidation;
using NNA.Domain.DTOs.DmoCollections;

namespace NNA.Api.Features.DmoCollections.Validators;

public sealed class AddDmoToCollectionDtoValidator : AbstractValidator<AddDmoToCollectionDto> {
    public AddDmoToCollectionDtoValidator() {
        RuleFor(d => d.CollectionId)
            .NotEmpty()
            .WithMessage("Collection id is missing");
        RuleFor(d => d.Dmos)
            .NotEmpty()
            .WithMessage("No dmo to add to collection");
    }
}