namespace NNA.Domain.DTOs.Characters;

public sealed class CreateCharacterDto : BaseDto {
    public string DmoId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Aliases { get; set; }
}
