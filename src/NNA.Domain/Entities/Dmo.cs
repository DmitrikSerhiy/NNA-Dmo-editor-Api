using NNA.Domain.Entities.Common;

namespace NNA.Domain.Entities;

public sealed class Dmo : Entity {
    public string Name { get; set; } = null!;
    public string MovieTitle { get; set; } = null!;
    public short DmoStatus { get; set; }
    public string? ShortComment { get; set; }
    public short? Mark { get; set; }
    public string? BeatsJson { get; set; }
    public bool HasBeats { get; set; }


    public NnaUser NnaUser { get; set; } = null!;
    public Guid NnaUserId { get; set; }

    public ICollection<DmoCollectionDmo> DmoCollectionDmos { get; set; } = new List<DmoCollectionDmo>();
    public ICollection<Beat> Beats { get; set; } = new List<Beat>();
    public ICollection<NnaMovieCharacter> Characters { get; set; } = new List<NnaMovieCharacter>();
}