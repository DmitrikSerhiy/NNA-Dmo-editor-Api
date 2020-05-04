using System;

namespace API.DTO.Dmos {
    public class BeatDto {
        public string Description { get; set; }
        public int Order { get; set; }
        public TimeDto PlotTimeSpot { get; set; }
    }
}
