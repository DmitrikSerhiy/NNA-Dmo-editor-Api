namespace NNA.Domain.DTOs.CharactersInBeats;

public sealed class AttachCharacterToBeatDto : BaseDto {
    public string DmoId { get; set; } = null!;
    public string BeatId { get; set; } = null!;
    public string CharacterId { get; set; } = null!;
}
