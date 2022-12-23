namespace NNA.Domain.DTOs.Community; 

public sealed class PublishedDmosDto: BaseDto {
    public ICollection<PublishedDmoShortDto> Dmos { get; set; } = new List<PublishedDmoShortDto>();
    public PaginationDetailsResultDto Pagination { get; set; } = new();
}