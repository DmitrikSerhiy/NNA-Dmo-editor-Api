namespace NNA.Domain.DTOs.Editor;

public sealed class BeatToSwapDto : BaseDto {
    // ReSharper disable InconsistentNaming

    public string id { get; set; } = null!;
    public int order { get; set; }
}
