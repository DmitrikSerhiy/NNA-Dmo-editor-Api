using FluentValidation;
using NNA.Domain.DTOs.DmoCollections;

namespace NNA.Api.Features.DmoCollections.Validators;

public sealed class DeleteCollectionDtoValidator : AbstractValidator<DeleteCollectionDto> {
    public DeleteCollectionDtoValidator() {
        RuleFor(d => d.CollectionId)
            .NotEmpty()
            .WithMessage("Collection id is missing");
    }
}