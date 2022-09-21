namespace NNA.Domain.DTOs.Editor;

public sealed class SwapBeatsDto: BaseDto {
    // ReSharper disable InconsistentNaming
    public string dmoId { get; set; } = null!;
    public BeatToSwapDto beatToMove { get; set; } = new();
    public BeatToSwapDto beatToReplace { get; set; } = new();
}
