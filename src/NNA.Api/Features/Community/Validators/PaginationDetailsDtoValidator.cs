using FluentValidation;
using NNA.Domain.DTOs;

namespace NNA.Api.Features.Community.Validators; 

public class PaginationDetailsDtoValidator : AbstractValidator<PaginationDetailsDto> {
    public PaginationDetailsDtoValidator() {
        RuleFor(p => p.PageNumber)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Not valid page number");
        
        RuleFor(p => p.PageSize)
            .NotEmpty()
            .WithMessage("Missing page size")
            .GreaterThanOrEqualTo(10)
            .WithMessage("Not valid page size");
    }
}