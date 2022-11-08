namespace NNA.Domain.DTOs.Characters;

public sealed class UpdateCharacterDto : BaseDto {
    public Guid Id { get; set; }
    public Guid DmoId { get; set; }
    public string Name { get; set; } = null!;
    public string? Aliases { get; set; }
    public string Color { get; set; } = null!;
}
