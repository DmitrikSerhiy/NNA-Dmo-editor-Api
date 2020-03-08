using API.DTO.DmoCollections;
using FluentValidation;

namespace API.Validators.DmoCollections
{
    // ReSharper disable UnusedMember.Global
    public class GetCollectionDtoValidator : AbstractValidator<GetCollectionDto> {
        public GetCollectionDtoValidator() {
            RuleFor(d => d.CollectionId).NotEmpty().WithMessage("Collection id is missing");
        }
    }
}
