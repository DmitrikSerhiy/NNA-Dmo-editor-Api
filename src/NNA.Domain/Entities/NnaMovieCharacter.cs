using NNA.Domain.Entities.Common;

namespace NNA.Domain.Entities;

public sealed class NnaMovieCharacter : Entity {
    public string Name { get; set; } = null!;
    public string? Aliases { get; set; }
    
    public Guid DmoId { get; set; }
    public Dmo Dmo { get; set; } = null!;

    public ICollection<Beat> Beats { get; set; } = new List<Beat>();
}
