using FluentValidation;
using Model;
using Model.DTOs.DmoCollections;

namespace API.Features.DmoCollections.Validators {
    // ReSharper disable UnusedMember.Global
    public class DmoCollectionShortDtoValidator : AbstractValidator<DmoCollectionShortDto> {
        public DmoCollectionShortDtoValidator() {
            RuleFor(d => d.Id).NotEmpty().WithMessage("Collection id is missing");
            RuleFor(d => d.CollectionName)
                .NotEmpty().WithMessage("Collection id is missing")
                .MaximumLength(ApplicationConstants.MaxEntityNameLength)
                .WithMessage($"Maximum collection name length is {ApplicationConstants.MaxEntityNameLength}");
        }
    }
}
