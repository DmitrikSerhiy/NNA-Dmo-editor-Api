using FluentValidation;
using NNA.Domain;
using NNA.Domain.DTOs.Dmos;

namespace NNA.Api.Features.Dmos.Validators;

public sealed class AddDmoByHttpDtoValidator: AbstractValidator<CreateDmoByHttpDto> {
    public AddDmoByHttpDtoValidator() {
        RuleFor(d => d.Name)
            .NotEmpty()
            .WithMessage("Dmo name is missing")
            .MaximumLength(ApplicationConstants.MaxDmoNameLength)
            .WithMessage($"Maximum dmo name length is {ApplicationConstants.MaxDmoNameLength}");

        RuleFor(d => d.MovieTitle)
            .NotEmpty()
            .WithMessage("Movie title is missing")
            .MaximumLength(ApplicationConstants.MaxMovieTitleLength)
            .WithMessage($"Maximum movie title length is {ApplicationConstants.MaxMovieTitleLength}");

        RuleFor(d => d.ShortComment)
            .MaximumLength(ApplicationConstants.MaxShortCommentLength)
            .WithMessage($"Maximum comment length is {ApplicationConstants.MaxShortCommentLength}");
    }
}
