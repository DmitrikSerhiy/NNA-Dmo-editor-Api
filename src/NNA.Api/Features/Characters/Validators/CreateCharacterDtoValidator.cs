using FluentValidation;
using NNA.Domain;
using NNA.Domain.DTOs.Characters;

namespace NNA.Api.Features.Characters.Validators;

public sealed class CreateCharacterDtoValidator: AbstractValidator<CreateCharacterDto> {
    public CreateCharacterDtoValidator() {
        RuleFor(cha => cha.DmoId)
            .NotEmpty()
            .WithMessage("Dmo id is missing");
        RuleFor(cha => cha.Name)
            .NotEmpty()
            .WithMessage("Character name is missing")
            .MaximumLength(ApplicationConstants.MaxCharacterNameLength)
            .WithMessage("Maximum character name length exceeded");
        RuleFor(cha => cha.Aliases)
            .MaximumLength(ApplicationConstants.MaxCharacterAliasesLength)
            .WithMessage("Maximum character aliases length exceeded");
        RuleFor(cha => cha.Color)
            .NotEmpty()
            .WithMessage("Color is missing");
    }
}
