﻿using FluentValidation;
using NNA.Domain;
using NNA.Domain.DTOs.Editor;

namespace NNA.Api.Features.Editor.Validators;

public sealed class UpdateShortDmoDtoValidator : AbstractValidator<UpdateShortDmoDto> {
    public UpdateShortDmoDtoValidator() {
        RuleFor(d => d.Id)
            .NotEmpty()
            .WithMessage("Dmo id is missing");

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
    }
}