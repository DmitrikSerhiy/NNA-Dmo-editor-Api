namespace NNA.Domain.DTOs.DmoCollections;

public sealed class DmoDetailsDto: BaseDto {
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string MovieTitle { get; set; } = null!;
    public string DmoStatus { get; set; } = null!;
    public short DmoStatusId { get; set; }
    public string ShortComment { get; set; } = null!;
}
