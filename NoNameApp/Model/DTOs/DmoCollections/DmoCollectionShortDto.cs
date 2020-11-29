using System;

namespace Model.DTOs.DmoCollections {
    public class DmoCollectionShortDto {
        public Guid? Id { get; set; }
        public string CollectionName { get; set; }
        public int DmoCount { get; set; }
    }
}
