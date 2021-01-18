using System;

namespace Model.Exceptions.Editor {
    public class UpdateDmoBeatsAsJsonException : Exception {
        private const string InnerMessage = "Failed to update dmos beats json.";
        public static string CustomMessage { get; } = InnerMessage;
        public UpdateDmoBeatsAsJsonException() { }

        public UpdateDmoBeatsAsJsonException(string message) : base($"{InnerMessage} {message}") { }

        public UpdateDmoBeatsAsJsonException(string message, Exception inner)
            : base($"{InnerMessage} {message}", inner) { }

        public UpdateDmoBeatsAsJsonException(Guid dmoId, Guid userId)
            : base($"{InnerMessage} DmoId: {dmoId}. UserId: {userId}") { }

        public UpdateDmoBeatsAsJsonException(Exception inner, Guid dmoId, Guid userId)
            : base($"{InnerMessage} DmoId: {dmoId}. UserId: {userId}", inner) { }
    }
}
