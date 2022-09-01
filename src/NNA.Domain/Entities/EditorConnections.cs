namespace NNA.Domain.Entities;

public sealed class EditorConnection {
    public string ConnectionId { get; set; } = null!;
    public Guid UserId { get; set; }
}