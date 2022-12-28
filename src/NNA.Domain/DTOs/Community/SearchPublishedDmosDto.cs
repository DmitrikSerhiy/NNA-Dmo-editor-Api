namespace NNA.Domain.DTOs.Community; 

public sealed class SearchPublishedDmosDto : PaginationDetailsDto {
    public string[] DmoIdsToIgnore { get; set; } = Array.Empty<string>();
    public int TotalAmount { get; set; }
}