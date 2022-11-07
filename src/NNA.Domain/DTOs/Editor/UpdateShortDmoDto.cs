namespace NNA.Domain.DTOs.Editor;

public sealed class UpdateShortDmoDto : BaseDto {
    public string? Id { get; set; }
    public string Name { get; set; } = null!;
    public string MovieTitle { get; set; } = null!;
    public string ShortComment { get; set; } = null!;
    public short DmoStatus { get; set; }
}