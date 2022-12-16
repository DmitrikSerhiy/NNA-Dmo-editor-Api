using NNA.Domain.DTOs.Beats;
using NNA.Domain.DTOs.Characters;
using NNA.Domain.DTOs.Tags;

namespace NNA.Domain.DTOs.Dmos;

public sealed class DmoWithDataDto : BaseDto {
    public ICollection<BeatDto> Beats { get; set; } = new List<BeatDto>();
    public ICollection<DmoCharacterDto> Characters { get; set; } = new List<DmoCharacterDto>();
    public ICollection<TagDto> Tags { get; set; } = new List<TagDto>();
}
