namespace NNA.Domain.Exceptions.Editor;

public class LoadShortDmoException : Exception {
    private const string InnerMessage = "Failed to load short dmo.";
    public static string CustomMessage { get; } = InnerMessage;
    public LoadShortDmoException() { }

    public LoadShortDmoException(string message) : base($"{InnerMessage} {message}") { }

    public LoadShortDmoException(string message, Exception inner)
        : base($"{InnerMessage} {message}", inner) { }

    public LoadShortDmoException(Guid dmoId, Guid userId)
        : base($"{InnerMessage} DmoId: {dmoId}. UserId: {userId}") { }

    public LoadShortDmoException(Exception inner, Guid dmoId, Guid userId)
        : base($"{InnerMessage} DmoId: {dmoId}. UserId: {userId}", inner) { }
}