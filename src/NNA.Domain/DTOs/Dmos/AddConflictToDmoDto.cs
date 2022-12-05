using NNA.Domain.Enums;

namespace NNA.Domain.DTOs.Dmos;

public sealed class AddConflictToDmoDto: BaseDto {
    public string CharacterId { get; set; } = null!;
    public CharacterType CharacterType { get; set; }
    public bool GoalAchieved { get; set; }
}
