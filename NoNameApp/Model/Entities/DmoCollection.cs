using System;
using System.Collections.Generic;
using Model.Entities.Common;

namespace Model.Entities {
    public sealed class DmoCollection : Entity {
        public string CollectionName { get; set; }

        public ICollection<DmoCollectionDmo> DmoCollectionDmos { get; set; }

        public Guid NnaUserId { get; set; }
        public NnaUser NnaUser { get; set; }

    }
}
