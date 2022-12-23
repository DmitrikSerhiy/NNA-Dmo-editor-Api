namespace NNA.Domain.DTOs.Community; 

public sealed class PublishedDmoShortDto : BaseDto {
    public string Id { get; set; } = null!;
    public string MovieTitle { get; set; } = null!;
    public string Name { get; set; } = null!;
    public short DmoStatusId { get; set; }
    public string AuthorNickname { get; set; } = null!;
    public string PublishDate { get; set; } = null!;
}