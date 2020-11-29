using System;

namespace Model.DTOs.DmoCollections
{
    public class RemoveDmoFromCollectionDto {
        public Guid? CollectionId { get; set; }
        public Guid? DmoId { get; set; }
    }
}
