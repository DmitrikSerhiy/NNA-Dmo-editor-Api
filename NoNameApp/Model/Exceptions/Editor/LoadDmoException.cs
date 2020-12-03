using System;

namespace Model.Exceptions.Editor {
    public class LoadDmoException : Exception {

        private const string InnerMessage = "Failed to load dmo.";
        public LoadDmoException() { }

        public LoadDmoException(string message) : base($"{InnerMessage} {message}") { }

        public LoadDmoException(string message, Exception inner)
            : base($"{InnerMessage} {message}", inner) { }

        public LoadDmoException(Guid dmoId, Guid userId)
            : base($"{InnerMessage} DmoId: {dmoId}. UserId: {userId}") { }

        public LoadDmoException(Exception inner, Guid dmoId, Guid userId)
            : base($"{InnerMessage} DmoId: {dmoId}. UserId: {userId}", inner) { }
    }
}
