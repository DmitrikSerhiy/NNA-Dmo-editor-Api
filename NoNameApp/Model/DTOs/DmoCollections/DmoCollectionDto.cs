namespace Model.DTOs.DmoCollections; 
public class DmoCollectionDto : BaseDto {
    public string Id { get; set; }
    public string CollectionName { get; set; }
    public int DmoCount { get; set; }
    public DmoShortDto[] Dmos { get; set; }
}