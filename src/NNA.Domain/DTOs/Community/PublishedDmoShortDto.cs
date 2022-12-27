namespace NNA.Domain.DTOs.Community; 

public sealed class PublishedDmoShortDto : BaseDto {
    public string Id { get; set; } = null!;
    public string MovieTitle { get; set; } = null!;
    public string? Name { get; set; }
    public short DmoStatusId { get; set; }
    public string AuthorNickname { get; set; } = null!;
    public string? ShortComment { get; set; }
    public string PublishDate { get; set; } = null!;
    public long PublishDateRaw { get; set; }
}