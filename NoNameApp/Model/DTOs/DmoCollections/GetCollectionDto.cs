using System;

namespace Model.DTOs.DmoCollections; 
public class GetCollectionDto : BaseDto {
    public Guid? CollectionId { get; set; }
}