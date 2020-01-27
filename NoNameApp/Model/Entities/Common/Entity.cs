using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Entities.Common {
    public class Entity {

        protected Entity() {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; private set; }
    }
}
