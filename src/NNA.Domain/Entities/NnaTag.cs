using NNA.Domain.Entities.Common;

namespace NNA.Domain.Entities;

public sealed class NnaTag: Entity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    
    public ICollection<NnaTagInBeat> Beats { get; set; } = new List<NnaTagInBeat>();

}