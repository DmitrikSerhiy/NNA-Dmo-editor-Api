namespace NNA.Domain.DTOs.DmoCollections;

public sealed class DmoCollectionDto : BaseDto {
    public string Id { get; set; } = null!;
    public string CollectionName { get; set; } = null!;
    public int DmoCount { get; set; }
    public DmoShortDto[] Dmos { get; set; } = Array.Empty<DmoShortDto>();
}