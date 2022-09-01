namespace NNA.Domain.Exceptions.Editor;

public class DeleteBeatException : Exception {
    private const string InnerMessage = "Failed to delete beat.";
    public static string CustomMessage { get; } = InnerMessage;

    public DeleteBeatException() { }

    public DeleteBeatException(string message) : base($"{InnerMessage} {message}") { }

    public DeleteBeatException(string message, Exception inner)
        : base($"{InnerMessage} {message}", inner) { }

    public DeleteBeatException(Guid dmoId, Guid userId)
        : base($"{InnerMessage} DmoId: {dmoId}. UserId: {userId}") { }

    public DeleteBeatException(Exception inner, Guid dmoId, Guid userId)
        : base($"{InnerMessage} DmoId: {dmoId}. UserId: {userId}", inner) { }
}