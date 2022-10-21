namespace NNA.Domain.DTOs.Characters;

public sealed class UpdateCharacterDto : BaseDto {
    public string DmoId { get; set; } = null!;
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Aliases { get; set; }
}
