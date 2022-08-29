using FluentValidation;
using NNA.Domain;
using NNA.Domain.DTOs.DmoCollections;

namespace NNA.Api.Features.DmoCollections.Validators;
public class DmoCollectionsValidator : AbstractValidator<DmoCollectionShortDto> {
    public DmoCollectionsValidator() {
        RuleFor(d => d.CollectionName)
            .NotEmpty()
            .WithMessage("Name of collection is missing")
            .MaximumLength(ApplicationConstants.MaxCollectionNameLength)
            .WithMessage($"Maximum collection name length is {ApplicationConstants.MaxCollectionNameLength}");
    }
}