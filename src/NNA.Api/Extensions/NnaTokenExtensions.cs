using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NNA.Api.Features.Account.Services;
using NNA.Domain.Entities;
using NNA.Domain.Models;

namespace NNA.Api.Extensions;

public static class NnaTokenExtensions {
    public static void AddSubjectClaims(this SecurityTokenDescriptor descriptor, NnaUser user) {
        var subject = new ClaimsIdentity();
        subject.AddClaim(new Claim(JwtRegisteredClaimNames.Email, user.Email));
        subject.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
        subject.AddClaim(new Claim(TokenValidationParametersProvider.NnaRoleClaimType, string.Join( ",", user.Roles)));
        descriptor.Subject = subject;
    }

    public static void AddSigningCredentials(this SecurityTokenDescriptor descriptor, JwtOptions options) {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key));
        descriptor.SigningCredentials = new SigningCredentials(key, options.SigningAlg);
    }
}