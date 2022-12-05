namespace NNA.Domain.DTOs.Dmos;

public sealed class DmoDetailsShortDto: BaseDto {
    public string MovieTitle { get; set; } = null!;
    public short DmoStatusId { get; set; }
}
