namespace NNA.Domain.DTOs.Editor;

public sealed class MoveBeatDto : BaseDto {
    public string dmoId { get; set; } = null!;
    public string id { get; set; } = null!;
    public int order { get; set; }
    public int previousOrder { get; set; }
}
