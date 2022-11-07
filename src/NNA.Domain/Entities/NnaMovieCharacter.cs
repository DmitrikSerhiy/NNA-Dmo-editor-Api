using NNA.Domain.Entities.Common;

namespace NNA.Domain.Entities;

public sealed class NnaMovieCharacter : Entity {
    public string Name { get; set; } = null!;
    public string? Aliases { get; set; }

    public string Color { get; set; } = "#000000";
    
    public Guid DmoId { get; set; }
    public Dmo Dmo { get; set; } = null!;

    public ICollection<NnaMovieCharacterInBeat> Beats { get; set; } = new List<NnaMovieCharacterInBeat>();
}
