using System;

namespace Model.Exceptions.Data {
    public class InconsistentDataException : Exception {
        
        public InconsistentDataException() { }

        public InconsistentDataException(string message) : base(message) { }
        
        public InconsistentDataException(string message, Exception inner) : base(message, inner) { }
    }
}