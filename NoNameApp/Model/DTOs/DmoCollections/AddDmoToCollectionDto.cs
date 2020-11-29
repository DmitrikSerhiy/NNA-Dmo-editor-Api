using System;

namespace Model.DTOs.DmoCollections {
    public class AddDmoToCollectionDto {
        public Guid? CollectionId { get; set; }
        public DmoInCollectionDto[] Dmos { get; set; }
    }
}
