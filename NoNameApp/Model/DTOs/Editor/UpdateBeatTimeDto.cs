﻿namespace Model.DTOs.Editor; 
public class UpdateBeatTimeDto: BaseDto {
    public int Hours { get; set; }
    public int Minutes { get; set; }
    public int Seconds { get; set; }
}