using FluentValidation;
using Model;
using Model.DTOs.DmoCollections;

namespace API.Features.DmoCollections.Validators {
    // ReSharper disable UnusedMember.Global
    public class AddNewDmoCollectionDtoValidator : AbstractValidator<AddNewDmoCollectionDto> {
        public AddNewDmoCollectionDtoValidator() {
            RuleFor(d => d.CollectionName)
                .NotEmpty().WithMessage("Collection id is missing")
                .MaximumLength(ApplicationConstants.MaxEntityNameLength)
                .WithMessage($"Maximum collection name length is {ApplicationConstants.MaxEntityNameLength}");
        }
    }
}
