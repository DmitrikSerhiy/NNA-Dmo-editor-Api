namespace NNA.Domain.DTOs.Dmos;

public sealed class CreatedDmoDto : BaseDto {
    public string Id { get; set; } = null!;
    public string MovieTitle { get; set; } = null!;
    public string? Name { get; set; }
    public string? ShortComment { get; set; }
}
