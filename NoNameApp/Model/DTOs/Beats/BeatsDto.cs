using System;

namespace Model.DTOs.Beats
{
    public class BeatsDto : BaseDto
    {
        public Guid DmoId { get; set; }
        public string BeatsJson { get; set; }
    }
}
