namespace NNA.Domain.DTOs.Editor;

public sealed class RemoveBeatDto : BaseDto {
    public string? Id { get; set; }
    public string? DmoId { get; set; }
    public int Order { get; set; }
}