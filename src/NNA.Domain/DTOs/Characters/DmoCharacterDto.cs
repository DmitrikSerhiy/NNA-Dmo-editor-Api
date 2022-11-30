namespace NNA.Domain.DTOs.Characters;

public sealed class DmoCharacterDto : BaseDto {
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Aliases { get; set; }
    public int Count { get; set; }
    public string Color { get; set; } = "#000000";
    public string? Goal { get; set; }
    public string? UnconsciousGoal { get; set; }
    public string[] Character { get; set; } = Array.Empty<string>();
    public string? Characterization { get; set; }
    public bool CharacterContradictsCharacterization { get; set; }
    public string? CharacterContradictsCharacterizationDescription { get; set; }
    public bool Emphathetic { get; set; }
    public string? EmphatheticDescription { get; set; }
    public bool Sympathetic { get; set; }
    public string? SympatheticDescription { get; set; }
}
