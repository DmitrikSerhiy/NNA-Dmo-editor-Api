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
            .WithMessage("Character's name is missing")
            .MaximumLength(60)
            .WithMessage("Maximum character's name length exceeded");
        RuleFor(cha => cha.Aliases)
            .MaximumLength(100)
            .WithMessage("Maximum character's aliases length exceeded");
        RuleFor(cha => cha.Color)
            .NotEmpty()
            .WithMessage("Color is missing");
    }
}
