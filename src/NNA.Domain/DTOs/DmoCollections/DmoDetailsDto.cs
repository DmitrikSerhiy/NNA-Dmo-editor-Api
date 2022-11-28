namespace NNA.Domain.DTOs.DmoCollections;

public sealed class DmoDetailsDto: BaseDto {
    public string Name { get; set; } = null!;
    public string MovieTitle { get; set; } = null!;
    public short DmoStatusId { get; set; }
    public string ShortComment { get; set; } = null!;
    // todo: extend for new popup
}
