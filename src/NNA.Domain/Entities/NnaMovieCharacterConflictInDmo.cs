using NNA.Domain.Entities.Common;

namespace NNA.Domain.Entities;

public sealed class NnaMovieCharacterConflictInDmo: Entity {
    public Guid PairId { get; set; }
    public short CharacterType { get; set; }
    public bool Achieved { get; set; }
    
    public Guid DmoId { get; set; }
    public Dmo Dmo { get; set; } = null!;
    
    public Guid? CharacterId { get; set; }
    public NnaMovieCharacter? Character { get; set; }
}
