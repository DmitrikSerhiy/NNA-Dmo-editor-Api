using FluentValidation;
using NNA.Domain.DTOs.Community;

namespace NNA.Api.Features.Community.Validators; 

public sealed class SearchPublishedDmosDtoValidator : AbstractValidator<SearchPublishedDmosDto> {
    public SearchPublishedDmosDtoValidator() {
        RuleFor(p => p.Amount)
            .LessThanOrEqualTo(5)
            .WithMessage("Not valid amount");
    }
}