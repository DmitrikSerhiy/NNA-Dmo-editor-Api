namespace NNA.Domain.DTOs.Dmos;

public sealed class DmoCharactersForConflictDto: BaseDto {
    public string CharacterId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Aliases { get; set; }
    public string Color { get; set; } = "#000000";
    public string? Goal { get; set; }
}
