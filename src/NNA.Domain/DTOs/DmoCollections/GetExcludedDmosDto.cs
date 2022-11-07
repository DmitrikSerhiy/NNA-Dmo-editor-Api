namespace NNA.Domain.DTOs.DmoCollections;

public sealed class GetExcludedDmosDto : BaseDto {
    public Guid CollectionId { get; set; }
    public bool Excluded { get; set; }
}