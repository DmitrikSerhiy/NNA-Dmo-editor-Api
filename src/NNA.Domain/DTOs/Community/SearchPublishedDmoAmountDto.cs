namespace NNA.Domain.DTOs.Community; 

public sealed class SearchPublishedDmoAmountDto : BaseDto {
    public string[] DmoIdsToIgnore { get; set; } = Array.Empty<string>();
}