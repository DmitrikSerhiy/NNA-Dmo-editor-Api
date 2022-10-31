namespace NNA.Domain.DTOs.CharactersInBeats;

public sealed class DetachCharacterToBeatDto: BaseDto {
    public string Id { get; set; } = null!;
    public string DmoId { get; set; } = null!;
    public string BeatId { get; set; } = null!;
}
