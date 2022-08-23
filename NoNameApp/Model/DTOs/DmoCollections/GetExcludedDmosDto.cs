using System;

namespace Model.DTOs.DmoCollections; 
public class GetExcludedDmosDto : BaseDto {
    public Guid? CollectionId { get; set; }
    public bool Excluded { get; set; }
}