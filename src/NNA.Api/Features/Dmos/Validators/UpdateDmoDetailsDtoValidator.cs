using FluentValidation;
using NNA.Domain;
using NNA.Domain.DTOs.Dmos;

namespace NNA.Api.Features.Dmos.Validators;

public sealed class UpdateDmoDetailsDtoValidator: AbstractValidator<UpdateDmoDetailsDto> {
    public UpdateDmoDetailsDtoValidator() {
        RuleFor(d => d.Name)
            .MaximumLength(ApplicationConstants.MaxEntityNameLength)
            .WithMessage($"Maximum dmo name length is {ApplicationConstants.MaxEntityNameLength}");

        RuleFor(d => d.MovieTitle)
            .NotEmpty()
            .WithMessage("Movie title is missing")
            .MaximumLength(ApplicationConstants.MaxMovieTitleLength)
            .WithMessage($"Maximum movie title length is {ApplicationConstants.MaxMovieTitleLength}");

        RuleFor(d => d.ShortComment)
            .MaximumLength(ApplicationConstants.MaxEntityNameLongLength)
            .WithMessage($"Maximum comment length is {ApplicationConstants.MaxEntityNameLongLength}");
        
        RuleFor(d => d.DmoStatusId)
            .IsInEnum()
            .WithMessage("Invalid status id");
    }
}
