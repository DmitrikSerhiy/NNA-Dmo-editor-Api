using System;

namespace Model.Exceptions.Editor
{
    public class CreateDmoException : Exception
    {
        public CreateDmoException() {
            throw new Exception("Failed to create exception");
        }

    }
}
