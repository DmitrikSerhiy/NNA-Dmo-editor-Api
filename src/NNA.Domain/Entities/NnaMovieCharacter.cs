using NNA.Domain.Entities.Common;

namespace NNA.Domain.Entities;

public sealed class NnaMovieCharacter : Entity {
    public string Name { get; set; } = null!;
    public string? Aliases { get; set; }

    public string Color { get; set; } = "#000000";
    
    public string? Goal { get; set; }
    public string? UnconsciousGoal { get; set; }
    public string? Characterization { get; set; }
    public bool CharacterContradictsCharacterization { get; set; }
    public string? CharacterContradictsCharacterizationDescription { get; set; }
    public bool Emphathetic { get; set; }
    public string? EmphatheticDescription { get; set; }
    public bool Sympathetic { get; set; }
    public string? SympatheticDescription { get; set; }
    
    public Guid DmoId { get; set; }
    public Dmo Dmo { get; set; } = null!;

    public ICollection<NnaMovieCharacterInBeat> Beats { get; set; } = new List<NnaMovieCharacterInBeat>();
    public ICollection<NnaMovieCharacterConflictInDmo> Conflicts { get; set; } = new List<NnaMovieCharacterConflictInDmo>();
}
