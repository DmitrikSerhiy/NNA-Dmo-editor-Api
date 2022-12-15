namespace NNA.Domain.DTOs.Tags;

public sealed class TagDto : BaseDto {
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}