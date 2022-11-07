namespace NNA.Domain.DTOs.DmoCollections;

public sealed class DmoCollectionShortDto : BaseDto {
    public Guid? Id { get; set; }
    public string CollectionName { get; set; } = null!;
    public int DmoCount { get; set; }
}