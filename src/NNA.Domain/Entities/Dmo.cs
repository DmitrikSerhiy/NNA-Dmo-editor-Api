using NNA.Domain.Entities.Common;

namespace NNA.Domain.Entities;

public sealed class Dmo : Entity {
    public string MovieTitle { get; set; } = null!;
    public string? Name { get; set; }
    public short DmoStatus { get; set; }
    public string? ShortComment { get; set; }
    public short? Mark { get; set; }
    public string? BeatsJson { get; set; }
    public bool HasBeats { get; set; }
    
    
    public string? Premise { get; set; }
    public string? ControllingIdea { get; set; }
    public short ControllingIdeaId { get; set; }
    public bool? Didacticism { get; set; }
    public string? DidacticismDescription { get; set; }
    
    public bool Published { get; set; }
    
    public NnaUser NnaUser { get; set; } = null!;
    public Guid NnaUserId { get; set; }

    public ICollection<DmoCollectionDmo> DmoCollectionDmos { get; set; } = new List<DmoCollectionDmo>();
    public ICollection<Beat> Beats { get; set; } = new List<Beat>();
    public ICollection<NnaMovieCharacter> Characters { get; set; } = new List<NnaMovieCharacter>();
    public ICollection<NnaMovieCharacterConflictInDmo> Conflicts { get; set; } = new List<NnaMovieCharacterConflictInDmo>();
}