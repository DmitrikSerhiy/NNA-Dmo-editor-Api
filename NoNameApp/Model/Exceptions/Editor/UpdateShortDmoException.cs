using System;

namespace Model.Exceptions.Editor {
    public class UpdateShortDmoException : Exception {
        private const string InnerMessage = "Failed to update short dmo.";
        public static string CustomMessage { get; } = InnerMessage;
        public UpdateShortDmoException() { }

        public UpdateShortDmoException(string message) : base($"{InnerMessage} {message}") { }

        public UpdateShortDmoException(string message, Exception inner)
            : base($"{InnerMessage} {message}", inner) { }

        public UpdateShortDmoException(Guid dmoId, Guid userId)
            : base($"{InnerMessage} DmoId: {dmoId}. UserId: {userId}") { }

        public UpdateShortDmoException(Exception inner, Guid dmoId, Guid userId)
            : base($"{InnerMessage} DmoId: {dmoId}. UserId: {userId}", inner) { }
    }
}
