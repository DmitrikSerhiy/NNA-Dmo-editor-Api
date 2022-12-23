namespace NNA.Domain.DTOs; 

public class PaginationDetailsDto : BaseDto {
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
}