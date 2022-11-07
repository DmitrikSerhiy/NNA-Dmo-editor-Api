namespace NNA.Domain.DTOs.Editor;

public sealed class CreateDmoDto : BaseDto {
    public string? Name { get; set; }
    public string MovieTitle { get; set; } = null!;
    public string ShortComment { get; set; } = null!;
    public short DmoStatus { get; set; }
}