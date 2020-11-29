namespace Model.DTOs.Dmos {
    public class BeatDto : BaseDto {
        public string Id { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public TimeDto PlotTimeSpot { get; set; }
    }
}
