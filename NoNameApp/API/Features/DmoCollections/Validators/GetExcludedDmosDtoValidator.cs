using FluentValidation;
using Model.DTOs.DmoCollections;

namespace API.Features.DmoCollections.Validators {
    // ReSharper disable UnusedMember.Global
    public class GetExcludedDmosDtoValidator : AbstractValidator<GetExcludedDmosDto> {
        public GetExcludedDmosDtoValidator() {
            RuleFor(d => d.CollectionId)
                .NotEmpty().WithMessage("Collection id is missing");
        }
    }
}
