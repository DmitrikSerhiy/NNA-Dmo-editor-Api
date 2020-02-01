using System;
using System.Collections.Generic;
using Model.Entities.Common;

namespace Model.Entities {
    public sealed class Dmo : Entity {
        public String Name { get; set; }
        public String MovieTitle { get; set; }

        public ICollection<DmoUserDmoCollection> DmoUserDmoCollections { get; set; }
    }
}
