namespace NNA.Domain.DTOs; 

public class PaginationDetailsResultDto : BaseDto {
    public int PageNumber { get; }
    public int TotalAmount { get; }
    public int PagesAmount { get; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < PagesAmount;

    public PaginationDetailsResultDto(int totalAmount, int pageNumber, int pageSize) {
        TotalAmount = totalAmount;
        PagesAmount = (int)Math.Ceiling(totalAmount / (double)pageSize);
        PageNumber = pageNumber;
    }

    public PaginationDetailsResultDto() {
        PageNumber = 0;
        TotalAmount = 0;
        PagesAmount = 0;
    }
}