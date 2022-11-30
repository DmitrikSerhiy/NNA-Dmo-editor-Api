namespace NNA.Domain.DTOs.Characters;

public sealed class UpdateCharacterDto : BaseDto {
    public Guid Id { get; set; }
    public Guid DmoId { get; set; }
    public string Name { get; set; } = null!;
    public string? Aliases { get; set; }
    public string Color { get; set; } = null!;
    public string? Goal { get; set; }
    public string? UnconsciousGoal { get; set; }
    public string? Characterization { get; set; }
    public bool CharacterContradictsCharacterization { get; set; }
    public string? CharacterContradictsCharacterizationDescription { get; set; }
    public bool Emphathetic { get; set; }
    public string? EmphatheticDescription { get; set; }
    public bool Sympathetic { get; set; }
    public string? SympatheticDescription { get; set; }
}
