using FluentValidation;
using NNA.Domain.DTOs.DmoCollections;

namespace NNA.Api.Features.DmoCollections.Validators;
public class GetExcludedDmosDtoValidator : AbstractValidator<GetExcludedDmosDto> {
    public GetExcludedDmosDtoValidator() {
        RuleFor(d => d.CollectionId)
            .NotEmpty()
            .WithMessage("Collection id is missing");
    }
}