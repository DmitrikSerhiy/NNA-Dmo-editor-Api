namespace API.DTO.Dmos {
    public class BeatDto {
        public string Id { get; set; }
        // ReSharper disable once UnusedMember.Global
        public string Description { get; set; }
        // ReSharper disable once UnusedMember.Global
        public int Order { get; set; }
        public TimeDto PlotTimeSpot { get; set; }
    }
}
