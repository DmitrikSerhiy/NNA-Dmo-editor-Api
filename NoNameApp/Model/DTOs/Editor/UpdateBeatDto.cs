namespace Model.DTOs.Editor {
    public class UpdateBeatDto {
        public string BeatId { get; set; }
        public string Text { get; set; }
        public UpdateBeatTimeDto Time { get; set; }
    }
}