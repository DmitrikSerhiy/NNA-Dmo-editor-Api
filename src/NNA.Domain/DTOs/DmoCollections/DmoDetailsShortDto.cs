namespace NNA.Domain.DTOs.DmoCollections;

public sealed class DmoDetailsShortDto: BaseDto {
    public string MovieTitle { get; set; } = null!;
    public short DmoStatusId { get; set; }
}
