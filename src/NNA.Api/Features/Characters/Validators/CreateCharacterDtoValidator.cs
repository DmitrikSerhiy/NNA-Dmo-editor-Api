﻿using FluentValidation;
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
        RuleFor(cha => cha.Goal)
            .MaximumLength(ApplicationConstants.MaxEntityNameLongLength)
            .WithMessage("Maximum goal length exceeded");
        RuleFor(cha => cha.UnconsciousGoal)
            .MaximumLength(ApplicationConstants.MaxEntityNameLongLength)
            .WithMessage("Maximum unconscious goal length exceeded");
        RuleFor(cha => cha.Characterization)
            .MaximumLength(ApplicationConstants.MaxEntityNameLongLength)
            .WithMessage("Maximum characterization length exceeded");
        RuleFor(cha => cha.CharacterContradictsCharacterizationDescription)
            .MaximumLength(ApplicationConstants.MaxEntityNameLongLength)
            .WithMessage("Maximum character contradicts characterization length exceeded");
        RuleFor(cha => cha.EmphatheticDescription)
            .MaximumLength(ApplicationConstants.MaxEntityNameLongLength)
            .WithMessage("Maximum emphathetic description length exceeded");
        RuleFor(cha => cha.SympatheticDescription)
            .MaximumLength(ApplicationConstants.MaxEntityNameLongLength)
            .WithMessage("Maximum sympathetic description length exceeded");
    }
}
