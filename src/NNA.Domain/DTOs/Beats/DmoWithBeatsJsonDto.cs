﻿namespace NNA.Domain.DTOs.Beats;

public sealed class DmoWithBeatsJsonDto : BaseDto {
    public Guid DmoId { get; set; }
    public string DmoStatus { get; set; } = null!;
    public short DmoStatusId { get; set; }
    public string BeatsJson { get; set; } = null!;
}