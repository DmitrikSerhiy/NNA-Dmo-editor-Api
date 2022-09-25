namespace NNA.Domain.Exceptions.Editor;

public sealed class SanitizeTempIdsException : Exception {
    private const string InnerMessage = "Failed to sanitize temp ids.";
    public static string CustomMessage { get; } = InnerMessage;
    public SanitizeTempIdsException() { }

    public SanitizeTempIdsException(string message) : base($"{InnerMessage} {message}") { }

    public SanitizeTempIdsException(string message, Exception inner)
        : base($"{InnerMessage} {message}", inner) { }

    public SanitizeTempIdsException(Guid dmoId, Guid userId)
        : base($"{InnerMessage} DmoId: {dmoId}. UserId: {userId}") { }

    public SanitizeTempIdsException(Exception inner, Guid dmoId, Guid userId)
        : base($"{InnerMessage} DmoId: {dmoId}. UserId: {userId}", inner) { }
}
