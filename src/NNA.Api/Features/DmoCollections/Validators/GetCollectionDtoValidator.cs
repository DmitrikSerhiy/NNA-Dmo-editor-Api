using FluentValidation;
using Model.DTOs.DmoCollections;

namespace NNA.Api.Features.DmoCollections.Validators;
public class GetCollectionDtoValidator : AbstractValidator<GetCollectionDto> {
    public GetCollectionDtoValidator() {
        RuleFor(d => d.CollectionId).NotEmpty().WithMessage("Collection id is missing");
    }
}