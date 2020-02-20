using System;

namespace API.DTO.DmoCollections {
    public class DmoCollectionDto {
        public String Id { get; set; }
        public String CollectionName { get; set; }
        public Int32 DmoCount { get; set; }
        public DmoShortDto[] Dmos { get; set; }
    }
}
