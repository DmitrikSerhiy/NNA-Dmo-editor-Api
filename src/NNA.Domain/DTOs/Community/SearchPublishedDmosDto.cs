namespace NNA.Domain.DTOs.Community; 

public sealed class SearchPublishedDmosDto : BaseDto {
    public string[] DmoIdsToIgnore { get; set; } = Array.Empty<string>();
    public int Amount { get; set; }
}