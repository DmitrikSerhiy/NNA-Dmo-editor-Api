using System;

namespace Model.Exceptions.Editor {
    public class InsertNewBeatException : Exception {
        private const string InnerMessage = "Failed to insert beat.";
        public static string CustomMessage { get; } = InnerMessage;
        
        public InsertNewBeatException() { }

        public InsertNewBeatException(string message) : base($"{InnerMessage} {message}") { }

        public InsertNewBeatException(string message, Exception inner)
            : base($"{InnerMessage} {message}", inner) { }

        public InsertNewBeatException(Guid dmoId, Guid userId)
            : base($"{InnerMessage} DmoId: {dmoId}. UserId: {userId}") { }

        public InsertNewBeatException(Exception inner, Guid dmoId, Guid userId)
            : base($"{InnerMessage} DmoId: {dmoId}. UserId: {userId}", inner) { }
    }
}