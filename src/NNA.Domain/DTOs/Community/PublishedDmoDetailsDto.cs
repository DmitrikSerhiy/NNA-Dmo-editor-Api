namespace NNA.Domain.DTOs.Community; 

public sealed class PublishedDmoDetailsDto : BaseDto {
    public string Id { get; set; } = null!;
    public int BeatsCount { get; set; }
    public int CharactersCount { get; set; }
    public string? Premise { get; set; }
    public string? ControllingIdea { get; set; }
    public int MinutesToRead { get; set; }
}