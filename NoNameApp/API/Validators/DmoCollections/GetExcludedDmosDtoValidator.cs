using API.DTO.DmoCollections;
using FluentValidation;

namespace API.Validators.DmoCollections {
    // ReSharper disable UnusedMember.Global
    public class GetExcludedDmosDtoValidator : AbstractValidator<GetExcludedDmosDto> {
        public GetExcludedDmosDtoValidator() {
            RuleFor(d => d.CollectionId)
                .NotEmpty().WithMessage("Collection id is missing");
        }
    }
}
