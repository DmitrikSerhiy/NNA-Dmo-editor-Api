using FluentValidation;
using NNA.Domain.DTOs.Characters;

namespace NNA.Api.Features.Characters.Validators;

public sealed class GetCharactersDtoValidator : AbstractValidator<GetCharactersDto> {
    public GetCharactersDtoValidator() {
        RuleFor(cha => cha.DmoId)
            .NotEmpty()
            .WithMessage("DmoId is missing");
    }
}
