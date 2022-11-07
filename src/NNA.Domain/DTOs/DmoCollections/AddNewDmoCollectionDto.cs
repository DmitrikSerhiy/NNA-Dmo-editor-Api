namespace NNA.Domain.DTOs.DmoCollections;

public sealed class AddNewDmoCollectionDto : BaseDto {
    public string CollectionName { get; set; } = null!;
}