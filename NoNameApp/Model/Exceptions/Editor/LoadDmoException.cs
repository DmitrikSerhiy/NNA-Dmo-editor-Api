using System;

namespace Model.Exceptions.Editor
{
    public class LoadDmoException : Exception
    {
        public LoadDmoException(Guid dmoId) {
            throw new Exception($"Failed to load dmo with id {dmoId}");
        }
    }
}
