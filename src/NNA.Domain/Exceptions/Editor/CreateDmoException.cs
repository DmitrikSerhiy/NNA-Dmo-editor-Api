﻿namespace NNA.Domain.Exceptions.Editor;

public sealed class CreateDmoException : Exception {
    private const string InnerMessage = "Failed to create dmo.";
    public static string CustomMessage { get; } = InnerMessage;
    public CreateDmoException() { }

    public CreateDmoException(string message) : base($"{InnerMessage} {message}") { }

    public CreateDmoException(string message, Exception inner)
        : base($"{InnerMessage} {message}", inner) { }

    public CreateDmoException(Guid dmoId, Guid userId)
        : base($"{InnerMessage} DmoId: {dmoId}. UserId: {userId}") { }

    public CreateDmoException(Exception inner, Guid dmoId, Guid userId)
        : base($"{InnerMessage} DmoId: {dmoId}. UserId: {userId}", inner) { }
}