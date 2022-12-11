using NNA.Domain.Enums;

namespace NNA.Domain.DTOs.Dmos;

public sealed class DmoConflictDto: BaseDto {
    public Guid Id { get; set; }
    public Guid PairId { get; set; }
    public string CharacterId { get; set; } = null!;
    public CharacterType CharacterType { get; set; }
    public bool Achieved { get; set; }
}
