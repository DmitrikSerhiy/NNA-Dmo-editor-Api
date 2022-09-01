using NNA.Domain.DTOs.Dmos;

namespace NNA.Api.Features.Editor.Services;

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