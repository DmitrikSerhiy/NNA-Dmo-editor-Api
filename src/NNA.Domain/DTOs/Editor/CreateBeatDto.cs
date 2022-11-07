namespace NNA.Domain.DTOs.Editor;

public sealed class CreateBeatDto : BaseDto {
    public string? TempId { get; set; }
    public int Order { get; set; }
    public string? DmoId { get; set; }
}