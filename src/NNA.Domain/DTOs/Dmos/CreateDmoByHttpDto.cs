namespace NNA.Domain.DTOs.Dmos;

public sealed class CreateDmoByHttpDto : BaseDto {
    public string Name { get; set; } = null!;
    public string MovieTitle { get; set; } = null!;
    public string? ShortComment { get; set; }
    public short DmoStatus { get; set; }
}
