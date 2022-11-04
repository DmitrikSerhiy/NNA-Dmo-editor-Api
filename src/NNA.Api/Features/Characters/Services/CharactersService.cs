using NNA.Domain;
using NNA.Domain.Entities;

namespace NNA.Api.Features.Characters.Services;

public sealed class CharactersService {
    public void SanitizeCharactersTempIdsInBeatDescription(Beat beat) {
        if (string.IsNullOrWhiteSpace(beat.Description)) {
            return;
        }
        if (beat.Characters.Count == 0) {
            return;
        }

        var charactersWithTempIds = beat.Characters
            .Where(cha => cha.TempId != null)
            .ToList();

        if (charactersWithTempIds.Count == 0) {
            return;
        }

        var beatDesc = Uri.UnescapeDataString(beat.Description);
        if (string.IsNullOrWhiteSpace(beatDesc)) {
            return;
        }

        foreach (var characterWithTempIds in charactersWithTempIds) {
            beatDesc = beatDesc.Replace(characterWithTempIds.TempId!, characterWithTempIds.Id.ToString());
            characterWithTempIds.TempId = null;
        }

        beat.Description = Uri.EscapeDataString(beatDesc);
    }
    
    public void SanitizeRemovedCharactersInBeatDescription(Beat beat, List<Guid> characterInBeatsIds) {
        if (string.IsNullOrWhiteSpace(beat.Description)) {
            return;
        }
        
        var beatDesc = Uri.UnescapeDataString(beat.Description);
        foreach (var characterInBeatId in characterInBeatsIds) {
            if (!beatDesc.Contains(characterInBeatId.ToString())) continue;
            beatDesc = beatDesc.Replace(Constants.NnaCharacterInterpolatorPrefix + characterInBeatId + Constants.NnaCharacterInterpolatorPostfix, "");
            beat.Description = Uri.EscapeDataString(beatDesc);
        }
    }
}
