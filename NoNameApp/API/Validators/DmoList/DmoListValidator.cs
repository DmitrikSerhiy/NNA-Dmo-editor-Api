using API.DTO;
using FluentValidation;
using Model;

namespace API.Validators.DmoList {
    // ReSharper disable once UnusedMember.Global
    public class DmoListValidator : AbstractValidator<DmoCollectionShortDto> {
        public DmoListValidator() {
            RuleFor(d => d.CollectionName)
                .NotEmpty().WithMessage("Name of collection is missing")
                .MaximumLength(ApplicationConstants.MaxEntityNameLength)
                .WithMessage($"Maximum collection name length is {ApplicationConstants.MaxEntityNameLength}");
        }
    }
}
