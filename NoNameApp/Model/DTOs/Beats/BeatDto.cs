namespace Model.DTOs.Beats {
    public class BeatDto : BaseDto {
        public string BeatId { get; set; }
        public string Text { get; set; }
        public string Order { get; set; }
        public BeatTimeDto Time { get; set; }
    }
}