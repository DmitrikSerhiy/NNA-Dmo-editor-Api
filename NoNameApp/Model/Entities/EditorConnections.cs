using System;

namespace Model.Entities; 
public sealed class EditorConnection {
    public string ConnectionId { get; set; }
    public Guid UserId { get; set; }
}