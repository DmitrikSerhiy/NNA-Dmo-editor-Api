using System;

namespace Model.Entities.Common {
    public class Entity {

        protected Entity() {
            Id = Guid.NewGuid();
            DateOfCreation = DateTimeOffset.UtcNow.UtcTicks;
        }

        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        public Guid Id { get; private set; }
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        public long DateOfCreation { get; private set; }
    }
}
