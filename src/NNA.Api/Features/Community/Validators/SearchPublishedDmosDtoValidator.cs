using FluentValidation;
using NNA.Domain.DTOs.Community;

namespace NNA.Api.Features.Community.Validators; 

public sealed class SearchPublishedDmosDtoValidator : AbstractValidator<SearchPublishedDmosDto> {
    public SearchPublishedDmosDtoValidator() {
        RuleFor(p => p.PageNumber)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Not valid page number");
        
        RuleFor(p => p.PageSize)
            .NotEmpty()
            .WithMessage("Missing page size")
            .GreaterThanOrEqualTo(10)
            .WithMessage("Not valid page size");
        
        RuleFor(p => p.TotalAmount)
            .NotEmpty()
            .WithMessage("Total amount is missing");
    }
}