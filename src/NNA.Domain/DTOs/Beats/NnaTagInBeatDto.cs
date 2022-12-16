namespace NNA.Domain.DTOs.Beats;

public sealed class NnaTagInBeatDto : BaseDto {
    public string Id { get; set; } = null!;
    public string TagId { get; set; } = null!;
    public string Name { get; set; } = null!;
}