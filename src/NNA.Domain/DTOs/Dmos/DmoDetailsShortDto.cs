namespace NNA.Domain.DTOs.Dmos;

public sealed class DmoDetailsShortDto: BaseDto {
    public string MovieTitle { get; set; } = null!;
    public bool Published { get; set; }
    public short DmoStatusId { get; set; }
}
