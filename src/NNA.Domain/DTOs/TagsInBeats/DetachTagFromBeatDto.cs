namespace NNA.Domain.DTOs.TagsInBeats;

public sealed class DetachTagFromBeatDto : BaseDto {
    public string Id { get; set; } = null!;
    public string DmoId { get; set; } = null!;
    public string BeatId { get; set; } = null!;
}