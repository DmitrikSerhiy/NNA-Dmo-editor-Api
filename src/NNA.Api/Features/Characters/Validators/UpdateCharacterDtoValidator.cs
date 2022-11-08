using FluentValidation;
using NNA.Domain.DTOs.Characters;

namespace NNA.Api.Features.Characters.Validators;

public sealed class UpdateCharacterDtoValidator : AbstractValidator<UpdateCharacterDto> {
    public UpdateCharacterDtoValidator() {
        RuleFor(cha => cha.DmoId)
            .NotEmpty()
            .WithMessage("Dmo id is missing");
        RuleFor(cha => cha.Id)
            .NotEmpty()
            .WithMessage("Character's id is missing");
        RuleFor(cha => cha.Name)
            .NotEmpty()
            .WithMessage("Character's name is missing");
        RuleFor(cha => cha.Color)
            .NotEmpty()
            .WithMessage("Color is missing");
    }
}
