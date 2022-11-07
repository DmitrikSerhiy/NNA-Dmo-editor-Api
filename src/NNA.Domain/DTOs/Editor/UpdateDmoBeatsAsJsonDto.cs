namespace NNA.Domain.DTOs.Editor;

public sealed class UpdateDmoBeatsAsJsonDto : BaseDto {
    public string? DmoId { get; set; }
    public string Data { get; set; } = null!;
}