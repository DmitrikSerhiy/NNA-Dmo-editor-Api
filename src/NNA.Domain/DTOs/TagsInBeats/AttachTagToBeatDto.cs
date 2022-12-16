namespace NNA.Domain.DTOs.TagsInBeats;

public sealed class AttachTagToBeatDto : BaseDto {
    public string Id { get; set; } = null!;
    public string DmoId { get; set; } = null!;
    public string BeatId { get; set; } = null!;
    public string TagId { get; set; } = null!;
}