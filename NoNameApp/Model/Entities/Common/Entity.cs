using System;

namespace Model.Entities.Common {
    public class Entity {

        protected Entity() {
            Id = Guid.NewGuid();
            DateOfCreation = DateTimeOffset.UtcNow.UtcTicks;
        }

        //todo: make these with privat set
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        public Guid Id { get; set; }
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        public long DateOfCreation { get; set; }
    }
}
