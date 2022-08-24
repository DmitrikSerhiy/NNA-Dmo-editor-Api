namespace NNA.Domain.Entities; 
public sealed class DmoCollectionDmo {
    public Guid DmoId { get; set; }
    public Dmo Dmo { get; set; }

    public Guid DmoCollectionId { get; set; }
    public DmoCollection DmoCollection { get; set; }
}