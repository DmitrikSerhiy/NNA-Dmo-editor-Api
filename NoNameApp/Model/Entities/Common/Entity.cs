using System;

namespace Model.Entities.Common {
    public class Entity {

        protected Entity() {
            Id = Guid.NewGuid();
            DateOfCreation = DateTimeOffset.UtcNow.UtcTicks;
        }

        public Guid Id { get; private set; }
        public Int64 DateOfCreation { get; private set; }
    }
}
