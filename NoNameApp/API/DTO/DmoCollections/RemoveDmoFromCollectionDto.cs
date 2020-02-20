using System;

namespace API.DTO.DmoCollections
{
    public class RemoveDmoFromCollectionDto {
        public Guid? CollectionId { get; set; }
        public Guid? DmoId { get; set; }
    }
}
