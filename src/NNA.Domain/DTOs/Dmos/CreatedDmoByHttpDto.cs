namespace NNA.Domain.DTOs.Dmos;

public class CreatedDmoByHttpDto : BaseDto {
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string MovieTitle { get; set; } = null!;
    public string? ShortComment { get; set; }
}
