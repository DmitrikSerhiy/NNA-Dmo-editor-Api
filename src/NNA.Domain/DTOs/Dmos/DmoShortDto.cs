namespace NNA.Domain.DTOs.Dmos;

public sealed class DmoShortDto : BaseDto {
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string MovieTitle { get; set; } = null!;
    public bool Published { get; set; }
    public string DmoStatus { get; set; } = null!;
    public short DmoStatusId { get; set; }
    public string ShortComment { get; set; } = null!;
    public short? Mark { get; set; }
}