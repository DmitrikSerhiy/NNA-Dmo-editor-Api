namespace NNA.Domain.Exceptions.Editor;

public class UpdateBeatException : Exception {
    private const string InnerMessage = "Failed to update beat.";
    public static string CustomMessage { get; } = InnerMessage;

    public UpdateBeatException() { }

    public UpdateBeatException(string message) : base($"{InnerMessage} {message}") { }

    public UpdateBeatException(string message, Exception inner)
        : base($"{InnerMessage} {message}", inner) { }

    public UpdateBeatException(string? beatId, Guid userId)
        : base($"{InnerMessage} BeatId: {beatId}. UserId: {userId}") { }

    public UpdateBeatException(Exception inner, string? beatId, Guid userId)
        : base($"{InnerMessage} BeatId: {beatId}. UserId: {userId}", inner) { }
}