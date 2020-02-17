using System;
using System.Collections.Generic;
using Model.Entities.Common;

namespace Model.Entities {
    public sealed class Dmo : Entity {
        public String Name { get; set; }
        public String MovieTitle { get; set; }
        public Int16 DmoStatus { get; set; }
        public String ShortComment { get; set; }
        public Int16 Mark { get; set; }


        public NoNameUser NoNameUser { get; set; }
        public Guid NoNameUserId { get; set; }

        public ICollection<DmoUserDmoCollection> DmoUserDmoCollections { get; set; }
    }
}
