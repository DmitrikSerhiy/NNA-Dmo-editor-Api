namespace NNA.Domain.DTOs.Dmos;

public sealed class UpdateDmoConflictDto: BaseDto {
    public bool Achieved { get; set; }
    public Guid? CharacterId { get; set; }
}
