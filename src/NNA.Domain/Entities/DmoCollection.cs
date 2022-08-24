using NNA.Domain.Entities.Common;

namespace NNA.Domain.Entities; 
public sealed class DmoCollection : Entity
{
    public string CollectionName { get; set; } = null!;

    public ICollection<DmoCollectionDmo> DmoCollectionDmos { get; set; } = new List<DmoCollectionDmo>();

    public Guid NnaUserId { get; set; }
    public NnaUser NnaUser { get; set; } = null!;

}