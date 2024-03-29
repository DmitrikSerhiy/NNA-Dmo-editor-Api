﻿using NNA.Domain.Enums;

namespace NNA.Domain.DTOs.Account;

public sealed class SendMailDto : BaseDto {
    public string Email { get; set; } = null!;
    public SendMailReason Reason { get; set; }
}