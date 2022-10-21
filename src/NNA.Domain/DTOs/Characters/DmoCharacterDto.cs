namespace NNA.Domain.DTOs.Characters;

public sealed class DmoCharacterDto : BaseDto {
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Aliases { get; set; }
}
