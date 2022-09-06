namespace NNA.Domain.DTOs.Dmos;

public class CreateDmoByHttpDto : BaseDto {
    public string Name { get; set; } = null!;
    public string MovieTitle { get; set; } = null!;
    public string? ShortComment { get; set; }
    public short DmoStatus { get; set; }
}
