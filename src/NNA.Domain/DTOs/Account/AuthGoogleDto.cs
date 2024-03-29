﻿namespace NNA.Domain.DTOs.Account;

public sealed class AuthGoogleDto : BaseDto {
    public string Email { get; set; } = null!;
    public string? Name { get; set; }
    public string GoogleToken { get; set; } = null!;
}