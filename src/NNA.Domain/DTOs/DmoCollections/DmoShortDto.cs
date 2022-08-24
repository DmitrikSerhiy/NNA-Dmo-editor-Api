namespace NNA.Domain.DTOs.DmoCollections; 
public sealed class DmoShortDto : BaseDto {
    public string Id { get; set; }
    public string Name { get; set; }
    public string MovieTitle { get; set; }
    public string DmoStatus { get; set; }
    public short DmoStatusId { get; set; }
    public string ShortComment { get; set; }
    public short? Mark { get; set; }
}