namespace NNA.Domain.Exceptions.Editor;

public sealed class RemoveCharacterFromBeatException : Exception {
    private const string InnerMessage = "Failed to remove character from beat.";
    public static string CustomMessage { get; } = InnerMessage;
    public RemoveCharacterFromBeatException() { }

    public RemoveCharacterFromBeatException(string message) : base($"{InnerMessage} {message}") { }

    public RemoveCharacterFromBeatException(string message, Exception inner)
        : base($"{InnerMessage} {message}", inner) { }

    public RemoveCharacterFromBeatException(Guid dmoId, Guid userId)
        : base($"{InnerMessage} DmoId: {dmoId}. UserId: {userId}") { }

    public RemoveCharacterFromBeatException(Exception inner, Guid dmoId, Guid userId)
        : base($"{InnerMessage} DmoId: {dmoId}. UserId: {userId}", inner) { }
    
    public RemoveCharacterFromBeatException(Exception inner, Guid dmoId, Guid userId, Guid beatId)
        : base($"{InnerMessage} DmoId: {dmoId}. UserId: {userId}. BeatId: {beatId}", inner) { }
}
