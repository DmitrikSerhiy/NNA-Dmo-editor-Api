using FluentValidation;
using NNA.Domain;
using NNA.Domain.DTOs.Dmos;

namespace NNA.Api.Features.Editor.Validators;

public sealed class UpdateDmoDetailsDtoValidator : AbstractValidator<UpdateDmoDetailsDto> {
    public UpdateDmoDetailsDtoValidator() {
        RuleFor(d => d.Id)
            .NotEmpty()
            .WithMessage("Dmo Id is missing");
        RuleFor(d => d.Name)
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
