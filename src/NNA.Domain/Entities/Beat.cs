using NNA.Domain.Entities.Common;
using NNA.Domain.Enums;

namespace NNA.Domain.Entities;

public sealed class Beat : Entity {
    public string? TempId { get; set; }
    public int BeatTime { get; set; }
    public string? BeatTimeView { get; set; }
    public string? Description { get; set; }
    public int Order { get; set; }
    public BeatType Type { get; set; }

    public Guid UserId { get; set; }
    public NnaUser User { get; set; } = null!;

    public Guid DmoId { get; set; }
    public Dmo Dmo { get; set; } = null!;
    public ICollection<NnaMovieCharacterInBeat> Characters { get; set; } = new List<NnaMovieCharacterInBeat>();
    public ICollection<NnaTagInBeat> Tags { get; set; } = new List<NnaTagInBeat>();

    
    
}