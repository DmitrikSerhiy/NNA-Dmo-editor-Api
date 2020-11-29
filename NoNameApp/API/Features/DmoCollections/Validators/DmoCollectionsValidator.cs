using FluentValidation;
using Model;
using Model.DTOs.DmoCollections;

namespace API.Features.DmoCollections.Validators {
    // ReSharper disable UnusedMember.Global
    public class DmoCollectionsValidator : AbstractValidator<DmoCollectionShortDto> {
        public DmoCollectionsValidator() {
            RuleFor(d => d.CollectionName)
                .NotEmpty().WithMessage("Name of collection is missing")
                .MaximumLength(ApplicationConstants.MaxEntityNameLength)
                .WithMessage($"Maximum collection name length is {ApplicationConstants.MaxEntityNameLength}");
        }
    }
}
