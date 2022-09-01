using NNA.Domain.Entities.Common;

namespace NNA.Domain.Entities;

public sealed class Beat : Entity {
    public string? TempId { get; set; }
    public int BeatTime { get; set; }
    public string? BeatTimeView { get; set; }
    public string? Description { get; set; }
    public int Order { get; set; }

    public Guid UserId { get; set; }
    public NnaUser User { get; set; } = null!;

    public Guid DmoId { get; set; }
    public Dmo Dmo { get; set; } = null!;
}