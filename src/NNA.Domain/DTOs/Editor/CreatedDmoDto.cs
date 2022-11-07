namespace NNA.Domain.DTOs.Editor;

public sealed class CreatedDmoDto : BaseDto {
    // ReSharper disable InconsistentNaming
    public string id { get; set; } = null!;
    public string name { get; set; } = null!;
    public string movieTitle { get; set; } = null!;
    public string shortComment { get; set; } = null!;
    public bool hasBeats { get; set; }
    public short dmoStatus { get; set; }
}