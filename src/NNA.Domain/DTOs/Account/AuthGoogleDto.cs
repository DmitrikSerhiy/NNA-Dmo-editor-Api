﻿namespace NNA.Domain.DTOs.Account; 
public class AuthGoogleDto: BaseDto
{
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string GoogleToken { get; set; } = null!;
}