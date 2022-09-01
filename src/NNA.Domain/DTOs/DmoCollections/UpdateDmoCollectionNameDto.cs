namespace NNA.Domain.DTOs.DmoCollections;

public sealed class UpdateDmoCollectionNameDto : BaseDto {
    public Guid? Id { get; set; }
    public string CollectionName { get; set; } = null!;
}
