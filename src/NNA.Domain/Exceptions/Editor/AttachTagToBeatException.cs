namespace NNA.Domain.Exceptions.Editor;

public sealed class AttachTagToBeatException : Exception {
    private const string InnerMessage = "Failed to attach tag to beat.";
    public static string CustomMessage { get; } = InnerMessage;
    
    public AttachTagToBeatException() { }

    public AttachTagToBeatException(string message) : base($"{InnerMessage} {message}") { }

    public AttachTagToBeatException(string message, Exception inner)
        : base($"{InnerMessage} {message}", inner) { }

    public AttachTagToBeatException(Guid dmoId, Guid userId)
        : base($"{InnerMessage} DmoId: {dmoId}. UserId: {userId}") { }

    public AttachTagToBeatException(Exception inner, Guid dmoId, Guid userId, Guid beatId, Guid tagId)
        : base($"{InnerMessage} DmoId: {dmoId}. UserId: {userId}. BeatId: {beatId}. TagId: {tagId} ", inner) { }
}