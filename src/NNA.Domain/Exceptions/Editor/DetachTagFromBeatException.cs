namespace NNA.Domain.Exceptions.Editor;

public sealed class DetachTagFromBeatException : Exception {
    private const string InnerMessage = "Failed to detach tag from beat.";
    public static string CustomMessage { get; } = InnerMessage;
    
    public DetachTagFromBeatException() { }

    public DetachTagFromBeatException(string message) : base($"{InnerMessage} {message}") { }

    public DetachTagFromBeatException(string message, Exception inner)
        : base($"{InnerMessage} {message}", inner) { }

    public DetachTagFromBeatException(Guid dmoId, Guid userId)
        : base($"{InnerMessage} DmoId: {dmoId}. UserId: {userId}") { }

    public DetachTagFromBeatException(Exception inner, Guid dmoId, Guid userId, Guid beatId)
        : base($"{InnerMessage} DmoId: {dmoId}. UserId: {userId}. BeatId: {beatId}.", inner) { }
}