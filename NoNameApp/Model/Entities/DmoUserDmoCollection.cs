using System;

namespace Model.Entities {
    public sealed class DmoUserDmoCollection
    {
        public Guid DmoId { get; set; }
        public Dmo Dmo { get; set; }

        public Guid UserDmoCollectionId { get; set; }
        public UserDmoCollection UserDmoCollection { get; set; }
    }
}
