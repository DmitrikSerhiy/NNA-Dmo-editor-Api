using NNA.Domain.Entities.Common;

namespace NNA.Domain.Entities;

public sealed class NnaMovieCharacterConflictInDmo: Entity {
    public Guid PairId { get; set; }
    public int PairOrder { get; set; }
    public short CharacterType { get; set; }
    public bool Achieved { get; set; }
    
    public Guid CharacterId { get; set; }
    public NnaMovieCharacter Character { get; set; } = null!;
}
