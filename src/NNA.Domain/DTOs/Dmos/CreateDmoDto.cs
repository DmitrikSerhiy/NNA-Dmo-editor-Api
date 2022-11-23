namespace NNA.Domain.DTOs.Dmos;

public sealed class CreateDmoDto : BaseDto {
    public string MovieTitle { get; set; } = null!;
    public string? Name { get; set; }
    public string? ShortComment { get; set; }
    public short DmoStatus { get; set; }
}
