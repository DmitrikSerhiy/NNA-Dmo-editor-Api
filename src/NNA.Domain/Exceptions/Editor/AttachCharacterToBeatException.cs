namespace NNA.Domain.Exceptions.Editor;

public sealed class AttachCharacterToBeatException : Exception {
    private const string InnerMessage = "Failed to attach character to beat.";
    public static string CustomMessage { get; } = InnerMessage;
    public AttachCharacterToBeatException() { }

    public AttachCharacterToBeatException(string message) : base($"{InnerMessage} {message}") { }

    public AttachCharacterToBeatException(string message, Exception inner)
        : base($"{InnerMessage} {message}", inner) { }

    public AttachCharacterToBeatException(Guid dmoId, Guid userId)
        : base($"{InnerMessage} DmoId: {dmoId}. UserId: {userId}") { }

    public AttachCharacterToBeatException(Exception inner, Guid dmoId, Guid userId, Guid beatId, Guid characterId)
        : base($"{InnerMessage} DmoId: {dmoId}. UserId: {userId}. BeatId: {beatId}. CharacterId: {characterId} ", inner) { }
}
