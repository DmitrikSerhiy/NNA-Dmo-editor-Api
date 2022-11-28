﻿using NNA.Domain.Enums;

namespace NNA.Domain.DTOs.Dmos;

public class UpdateDmoDetailsDto : BaseDto {
    public string MovieTitle { get; set; } = null!;
    public DmoStatus DmoStatusId { get; set; }
    public string? Name { get; set; }
    public string? ShortComment { get; set; }
}
