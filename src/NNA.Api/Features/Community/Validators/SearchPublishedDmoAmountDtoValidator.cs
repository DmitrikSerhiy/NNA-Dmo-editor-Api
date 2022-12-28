using FluentValidation;
using NNA.Domain.DTOs.Community;

namespace NNA.Api.Features.Community.Validators; 

public sealed class SearchPublishedDmoAmountDtoValidator : AbstractValidator<SearchPublishedDmoAmountDto> {
    public SearchPublishedDmoAmountDtoValidator() {
        
    }
}