using FluentValidation;
using NNA.Domain.DTOs.Characters;

namespace NNA.Api.Features.Characters.Validators;

public sealed class CreateCharacterDtoValidator: AbstractValidator<CreateCharacterDto> {
    public CreateCharacterDtoValidator() {
        RuleFor(cha => cha.DmoId)
            .NotEmpty()
            .WithMessage("Dmo id is missing");
        RuleFor(cha => cha.Name)
            .NotEmpty()
            .WithMessage("Character's name is missing");
    }
}
