using System;
using System.Collections.Generic;
using Model.Entities.Common;

namespace Model.Entities {
    public class UserDmoCollection : Entity {
        public String CollectionName { get; set; }
        public Int32 DmoCount { get; set; }
        public ICollection<Dmo> Dmos { get; set; }

        public ICollection<DmoUserDmoCollection> DmoUserDmoCollections { get; set; }

        public Guid NoNameUserId { get; set; }
        public NoNameUser NoNameUser { get; set; }

    }
}
