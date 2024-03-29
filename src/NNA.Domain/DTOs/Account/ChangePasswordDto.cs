﻿namespace NNA.Domain.DTOs.Account;

public sealed class ChangePasswordDto : BaseDto {
    public string Email { get; set; } = null!;
    public string CurrentPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}