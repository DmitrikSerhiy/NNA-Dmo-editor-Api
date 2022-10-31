using NNA.Domain.Entities.Common;

namespace NNA.Domain.Entities;

public sealed class NnaMovieCharacterInBeat : Entity {
    public string? TempId { get; set; }
    public Guid BeatId { get; set; }
    public Beat Beat { get; set; } = null!;
    public Guid CharacterId { get; set; }
    public NnaMovieCharacter Character { get; set; } = null!;
}
