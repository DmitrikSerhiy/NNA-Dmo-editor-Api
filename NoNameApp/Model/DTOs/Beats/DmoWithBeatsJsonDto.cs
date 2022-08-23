using System;

namespace Model.DTOs.Beats; 
public class DmoWithBeatsJsonDto : BaseDto
{
    public Guid DmoId { get; set; }
    public string DmoStatus { get; set; }
    public short DmoStatusId { get; set; }
    public string BeatsJson { get; set; }
}