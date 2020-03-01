using System;

namespace API.DTO.DmoCollections {
    public class AddDmoToCollectionDto {
        public Guid? CollectionId { get; set; }
        public DmoInCollectionDto[] Dmos { get; set; }
    }
}
