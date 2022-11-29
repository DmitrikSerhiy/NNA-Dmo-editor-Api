using FluentValidation;
using NNA.Domain;
using NNA.Domain.DTOs.Dmos;

namespace NNA.Api.Features.Editor.Validators;

public sealed class UpdateDmoPlotDetailsDtoValidator: AbstractValidator<UpdateDmoPlotDetailsDto> {
    public UpdateDmoPlotDetailsDtoValidator() {
        RuleFor(d => d.Premise)
            .MaximumLength(ApplicationConstants.MaxEntityNameLongLength)
            .WithMessage($"Maximum premise length is {ApplicationConstants.MaxEntityNameLongLength}");
        
        RuleFor(d => d.ControllingIdea)
            .MaximumLength(ApplicationConstants.MaxEntityNameLongLength)
            .WithMessage($"Maximum controlling idea length is {ApplicationConstants.MaxEntityNameLongLength}");
        
        RuleFor(d => d.DidacticismDescription)
            .MaximumLength(ApplicationConstants.MaxEntityNameLongLength)
            .WithMessage($"Maximum didacticism description idea length is {ApplicationConstants.MaxEntityNameLongLength}");

        RuleFor(d => d.ControllingIdeaId)
            .IsInEnum()
            .WithMessage("Invalid control idea id");
    }
}
