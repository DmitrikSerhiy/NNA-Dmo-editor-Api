using NNA.Domain.Entities.Common;

namespace NNA.Domain.Entities;

public sealed class NnaTagInBeat : Entity
{
    public string? TempId { get; set; }
    public Guid BeatId { get; set; }
    public Beat Beat { get; set; } = null!;
    
    public Guid TagId { get; set; }
    public NnaTag Tag { get; set; } = null!;
}