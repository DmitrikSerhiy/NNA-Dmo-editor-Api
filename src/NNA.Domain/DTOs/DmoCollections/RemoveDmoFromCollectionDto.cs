namespace NNA.Domain.DTOs.DmoCollections;

public sealed class RemoveDmoFromCollectionDto : BaseDto {
    public Guid CollectionId { get; set; }
    public Guid DmoId { get; set; }
}