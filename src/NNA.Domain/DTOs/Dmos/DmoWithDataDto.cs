using NNA.Domain.DTOs.Beats;
using NNA.Domain.DTOs.Characters;

namespace NNA.Domain.DTOs.Dmos;

public sealed class DmoWithDataDto : BaseDto {
    public ICollection<BeatDto> Beats { get; set; } = new List<BeatDto>();
    public ICollection<DmoCharacterDto> Characters { get; set; } = new List<DmoCharacterDto>();
}
