using FluentValidation;
using NNA.Domain;
using NNA.Domain.DTOs.DmoCollections;

namespace NNA.Api.Features.DmoCollections.Validators;

public sealed class AddNewDmoCollectionDtoValidator : AbstractValidator<AddNewDmoCollectionDto> {
    public AddNewDmoCollectionDtoValidator() {
        RuleFor(d => d.CollectionName)
            .NotEmpty()
            .WithMessage("Collection id is missing")
            .MaximumLength(ApplicationConstants.MaxCollectionNameLength)
            .WithMessage($"Maximum collection name length is {ApplicationConstants.MaxCollectionNameLength}");
    }
}