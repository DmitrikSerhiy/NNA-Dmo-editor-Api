namespace NNA.Domain.DTOs.Dmos;

public sealed class AddConflictPairToDmoDto: BaseDto {
    public int Order { get; set; }
    public AddConflictToDmoDto Protagonist { get; set; } = null!;
    public AddConflictToDmoDto Antagonist { get; set; } = null!;

}
