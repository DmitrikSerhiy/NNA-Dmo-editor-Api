using NNA.Domain;
using NNA.Domain.Entities;

namespace NNA.Api.Features.Characters.Services;

public sealed class TempIdSanitizer {
    public void SanitizeTagsTempIdsInBeatDescription(Beat beat) {
        if (string.IsNullOrWhiteSpace(beat.Description)) {
            return;
        }
        
        if (beat.Tags.Count == 0) {
            return;
        }
        
        var tagsWithTempIds = beat.Tags
            .Where(t => t.TempId != null)
            .ToList();
        
        if (tagsWithTempIds.Count == 0) {
            return;
        }
        
        var beatDesc = Uri.UnescapeDataString(beat.Description);
        if (string.IsNullOrWhiteSpace(beatDesc)) {
            return;
        }
        
        foreach (var tagWithTempId in tagsWithTempIds) {
            beatDesc = beatDesc.Replace(tagWithTempId.TempId!, tagWithTempId.Id.ToString());
            tagWithTempId.TempId = null;
        }

        beat.Description = Uri.EscapeDataString(beatDesc);
    }
    
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
            beatDesc = beatDesc.Replace(ApplicationConstants.NnaCharacterInterpolatorPrefix + characterInBeatId + ApplicationConstants.NnaCharacterInterpolatorPostfix, "");
            beat.Description = Uri.EscapeDataString(beatDesc);
        }
    }
}
