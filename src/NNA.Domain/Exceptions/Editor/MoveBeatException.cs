namespace NNA.Domain.Exceptions.Editor;

public sealed class MoveBeatException : Exception {
    private const string InnerMessage = "Failed to move beat.";
    public static string CustomMessage { get; } = InnerMessage;
    public MoveBeatException() { }

    public MoveBeatException(string message) : base($"{InnerMessage} {message}") { }

    public MoveBeatException(string message, Exception inner)
        : base($"{InnerMessage} {message}", inner) { }

    public MoveBeatException(Guid dmoId, Guid userId, string beatId)
        : base($"{InnerMessage} DmoId: {dmoId}. UserId: {userId}. BeatId: {beatId}") { }

    public MoveBeatException(Exception inner, Guid dmoId, Guid userId, string beatId)
        : base($"{InnerMessage} DmoId: {dmoId}. UserId: {userId}. BeatId: {beatId}", inner) { }
}
