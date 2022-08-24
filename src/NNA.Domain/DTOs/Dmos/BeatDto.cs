namespace NNA.Domain.DTOs.Dmos; 
public class BeatDto : BaseDto {
    public string Id { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int Order { get; set; }
}