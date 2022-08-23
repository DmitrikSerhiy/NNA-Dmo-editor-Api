namespace Model.DTOs.Editor; 
public class CreateDmoDto : BaseDto {
    public string Name { get; set; }
    public string MovieTitle { get; set; }
    public string ShortComment { get; set; }
    public short DmoStatus { get; set; }
}