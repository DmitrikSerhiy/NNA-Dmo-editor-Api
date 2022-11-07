﻿namespace NNA.Domain.DTOs.Beats;

public sealed class BeatTimeDto : BaseDto {
    public int Hours { get; set; }
    public int Minutes { get; set; }
    public int Seconds { get; set; }
}