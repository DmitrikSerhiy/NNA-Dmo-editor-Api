using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Model.Models;

namespace NNA.Api.Extensions; 

public static class NnaToken {
    public static void AddSubjectClaims(this SecurityTokenDescriptor descriptor, string userEmail, Guid userGuid) {
        var subject = new ClaimsIdentity();
        subject.AddClaim(new Claim(JwtRegisteredClaimNames.Email, userEmail));
        subject.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, userGuid.ToString()));
        descriptor.Subject = subject;
    }
        
    public static void AddSigningCredentials(this SecurityTokenDescriptor descriptor, JwtOptions options) {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key));
        descriptor.SigningCredentials = new SigningCredentials(key, options.SigningAlg);
    }
}