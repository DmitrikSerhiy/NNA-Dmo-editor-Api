namespace NNA.Domain.DTOs.Dmos;

public sealed class SanitizeInterpolatedCharacterInBeatsDto : BaseDto {
    public ICollection<string> CharacterIds { get; set; } = new List<string>();

}
