using System;

namespace API.DTO.Dmos {
    public class PartialDmoWithBeatsDto {
        public String DmoId { get; set; }
        public BeatDto[] Beats { get; set; }
    }
}
