using System;

namespace Model.DTOs.Dmos; 
public class RemoveDmoDto : BaseDto {
    public Guid? DmoId { get; set; }
}