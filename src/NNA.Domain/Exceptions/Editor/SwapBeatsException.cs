namespace NNA.Domain.Exceptions.Editor;

public sealed class SwapBeatsException : Exception {
    private const string InnerMessage = "Failed to swap beats.";
    public static string CustomMessage { get; } = InnerMessage;
    public SwapBeatsException() { }

    public SwapBeatsException(string message) : base($"{InnerMessage} {message}") { }

    public SwapBeatsException(string message, Exception inner)
        : base($"{InnerMessage} {message}", inner) { }

    public SwapBeatsException(Guid dmoId, Guid userId)
        : base($"{InnerMessage} DmoId: {dmoId}. UserId: {userId}") { }

    public SwapBeatsException(Exception inner, Guid dmoId, Guid userId)
        : base($"{InnerMessage} DmoId: {dmoId}. UserId: {userId}", inner) { }
}
