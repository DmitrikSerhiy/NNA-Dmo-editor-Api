using System;

namespace API.DTO.Dmos {
    public class BeatDto {
        public String Id { get; set; }
        public String Description { get; set; }
        public int Order { get; set; }
        public TimeDto PlotTimeSpot { get; set; }
    }
}
