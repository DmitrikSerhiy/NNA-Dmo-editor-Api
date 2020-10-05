using API.DTO.Dmos;
using System;
using System.Linq;

namespace API.Hubs.Helpers {
    public class BeatsIdGenerator {
        public static BeatDto[] GenerateMissingBeatsIds(BeatDto[] beats) {
            return beats.Select(beat => {
                if (string.IsNullOrWhiteSpace(beat.Id)) {
                    beat.Id = Guid.NewGuid().ToString();
                }
                return beat;
            })
            .ToArray();
        }
    }
}
