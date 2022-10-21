using FluentValidation;
using NNA.Domain.DTOs.Dmos;

namespace NNA.Api.Features.Dmos.Validators;

public sealed class GetDmoWithDataDtoValidator : AbstractValidator<GetDmoWithDataDto> {
    public GetDmoWithDataDtoValidator() {
        RuleFor(cha => cha.Id)
            .NotEmpty()
            .WithMessage("Dmo id is missing");
    }
}
