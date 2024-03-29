﻿using Microsoft.IdentityModel.Tokens;
using NNA.Api.Extensions;
using NNA.Domain.Entities;
using NNA.Domain.Enums;
using NNA.Domain.Models;

namespace NNA.Api.Features.Account.Services;

public sealed class TokenDescriptorProvider {
    private readonly JwtOptions _jwtOptions;

    public TokenDescriptorProvider(JwtOptions jwtOptions) {
        _jwtOptions = jwtOptions ?? throw new ArgumentNullException(nameof(jwtOptions));
    }

    public SecurityTokenDescriptor ProvideForAccessTokenWithCredentialsAndSubject(NnaUser user) {
        var accessTokenDescriptor = ProvideForAccessToken();
        accessTokenDescriptor.AddSigningCredentials(_jwtOptions);
        accessTokenDescriptor.AddSubjectClaims(user);

        return accessTokenDescriptor;
    }

    public SecurityTokenDescriptor ProvideForRefreshTokenWithCredentialsAndSubject(NnaUser user) {
        var refreshTokenDescriptor = ProvideForRefreshToken();
        refreshTokenDescriptor.AddSigningCredentials(_jwtOptions);
        refreshTokenDescriptor.AddSubjectClaims(user);

        return refreshTokenDescriptor;
    }

    public SecurityTokenDescriptor ProvideForAccessToken() {
        return BuildDescriptor();
    }

    public SecurityTokenDescriptor ProvideForRefreshToken() {
        var descriptor = BuildDescriptor();
        descriptor.Expires = DateTime.UtcNow + TimeSpan.FromHours(_jwtOptions.TokenLifetimeInHours * 2);
        descriptor.Claims.Add(
            nameof(NnaCustomTokenClaims.gtyp),
            NnaCustomTokenClaimsDictionary.GetValue(NnaCustomTokenClaims.gtyp));
        return descriptor;
    }

    private SecurityTokenDescriptor BuildDescriptor() {
        return new SecurityTokenDescriptor {
            Audience = _jwtOptions.Audience,
            Expires = DateTime.UtcNow + TimeSpan.FromHours(_jwtOptions.TokenLifetimeInHours),
            Issuer = _jwtOptions.Issuer,
            NotBefore = DateTime.UtcNow,
            IssuedAt = DateTime.UtcNow,
            TokenType = "JWT",
            Claims = new Dictionary<string, object> {
                { nameof(NnaCustomTokenClaims.oid), Guid.NewGuid() }
            }
        };
    }
}