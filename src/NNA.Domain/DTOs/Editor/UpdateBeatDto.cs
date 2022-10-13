using NNA.Domain.Enums;

namespace NNA.Domain.DTOs.Editor;

public class UpdateBeatDto : BaseDto {
    public string? BeatId { get; set; }
    public string Text { get; set; } = null!;
    public BeatType Type { get; set; }
    public UpdateBeatTimeDto Time { get; set; } = null!;
}