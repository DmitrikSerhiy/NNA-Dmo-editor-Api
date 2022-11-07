namespace NNA.Domain.Exceptions.Data;

public sealed class InconsistentDataException : Exception {
    public InconsistentDataException() { }

    public InconsistentDataException(string message) : base(message) { }

    public InconsistentDataException(string message, Exception inner) : base(message, inner) { }
}