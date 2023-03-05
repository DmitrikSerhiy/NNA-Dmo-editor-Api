using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NNA.Domain.Entities;

namespace NNA.Api.Features.Account.Services; 

public sealed class NnaDataProtectorTokenProvider : DataProtectorTokenProvider<NnaUser> {
    private const string NnaTokenProviderName = "NnaDataProtectorTokenProvider";
    
    public NnaDataProtectorTokenProvider(
        IDataProtectionProvider dataProtectionProvider,
        IOptions<DataProtectionTokenProviderOptions> options, 
        ILogger<DataProtectorTokenProvider<NnaUser>> logger) 
        : base(dataProtectionProvider, options, logger) {

        options.Value.TokenLifespan = TimeSpan.FromHours(1);
        options.Value.Name = NnaTokenProviderName;
    }
}