using System;
using System.Collections.Generic;
using Model.Entities.Common;

namespace Model.Entities {
    public sealed class Dmo : Entity {
        public string Name { get; set; }
        public string MovieTitle { get; set; }
        public short DmoStatus { get; set; }
        public string ShortComment { get; set; }
        public short Mark { get; set; }


        public NoNameUser NoNameUser { get; set; }
        public Guid NoNameUserId { get; set; }

        public ICollection<DmoUserDmoCollection> DmoUserDmoCollections { get; set; }

        public ICollection<Beat> Beats { get; set; }
        public string BeatsJson { get; set; }
    }
}
