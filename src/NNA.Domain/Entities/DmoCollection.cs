using NNA.Domain.Entities.Common;

namespace NNA.Domain.Entities; 
public sealed class DmoCollection : Entity {
    public string CollectionName { get; set; }

    public ICollection<DmoCollectionDmo> DmoCollectionDmos { get; set; }

    public Guid NnaUserId { get; set; }
    public NnaUser NnaUser { get; set; }

}