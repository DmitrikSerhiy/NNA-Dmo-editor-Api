namespace NNA.Domain.DTOs.DmoCollections;

public class AddDmoToCollectionDto : BaseDto {
    public Guid? CollectionId { get; set; }
    public DmoInCollectionDto[] Dmos { get; set; } = Array.Empty<DmoInCollectionDto>();
}