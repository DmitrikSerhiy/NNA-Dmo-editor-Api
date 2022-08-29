using FluentValidation;
using NNA.Domain.DTOs.DmoCollections;

namespace NNA.Api.Features.DmoCollections.Validators;
public class DmoInCollectionDtoValidator : AbstractValidator<DmoInCollectionDto> {
    public DmoInCollectionDtoValidator() {
        RuleFor(d => d.Id)
            .NotEmpty()
            .WithMessage("Dmo id is missing");
    }
}