using System;

namespace Model.DTOs.DmoCollections; 
public class DeleteCollectionDto : BaseDto {
    public Guid? CollectionId { get; set; }
}