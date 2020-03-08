using API.DTO.DmoCollections;
using FluentValidation;
using Model;

namespace API.Validators.DmoCollections {
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
