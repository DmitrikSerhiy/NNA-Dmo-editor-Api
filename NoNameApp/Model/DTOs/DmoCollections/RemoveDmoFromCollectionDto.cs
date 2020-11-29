using System;

namespace Model.DTOs.DmoCollections
{
    public class RemoveDmoFromCollectionDto : BaseDto {
        public Guid? CollectionId { get; set; }
        public Guid? DmoId { get; set; }
    }
}
