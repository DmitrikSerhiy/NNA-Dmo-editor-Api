namespace Model.DTOs.Editor {
    public class CreateBeatDto: BaseDto {
        public string TempId { get; set; }
        public int Order { get; set; }
        public string DmoId { get; set; }
        
    }
}