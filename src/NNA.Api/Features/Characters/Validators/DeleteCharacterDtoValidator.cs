using FluentValidation;
using NNA.Domain.DTOs.Characters;

namespace NNA.Api.Features.Characters.Validators;

public sealed class DeleteCharacterDtoValidator : AbstractValidator<DeleteCharacterDto> {
    public DeleteCharacterDtoValidator() {
        RuleFor(cha => cha.Id)
            .NotEmpty()
            .WithMessage("Character id is missing");
    }
}
