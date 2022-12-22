using FluentValidation;
using NNA.Domain.DTOs.Dmos;

namespace NNA.Api.Features.Dmos.Validators;

public sealed class PublishOrUnpublishDmoDtoValidator : AbstractValidator<PublishOrUnpublishDmoDto> {
    public PublishOrUnpublishDmoDtoValidator() {
        RuleFor(d => d.State)
            .IsInEnum()
            .WithMessage("State is not valid");
    }
}