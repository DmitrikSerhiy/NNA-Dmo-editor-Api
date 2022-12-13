using NNA.Domain.Enums;

namespace NNA.Domain.DTOs.Dmos;

public sealed class PatchDmoDetailsDto: BaseDto {
    public string MovieTitle { get; set; } = null!;
    public DmoStatus DmoStatusId { get; set; }
    public string? Name { get; set; }
    public string? ShortComment { get; set; }
}
