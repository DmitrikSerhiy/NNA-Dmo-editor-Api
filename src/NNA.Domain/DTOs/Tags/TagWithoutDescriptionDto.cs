namespace NNA.Domain.DTOs.Tags;

public sealed class TagWithoutDescriptionDto : BaseDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
}