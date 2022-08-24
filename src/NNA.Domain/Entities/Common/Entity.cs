namespace NNA.Domain.Entities.Common; 
public class Entity {
    protected Entity() {
        Id = Guid.NewGuid();
        DateOfCreation = DateTimeOffset.UtcNow.UtcTicks;
    }

    public Guid Id { get; private set; }

    public long DateOfCreation { get; private set; }
}