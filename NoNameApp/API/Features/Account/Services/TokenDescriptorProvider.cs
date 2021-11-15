using System;
using System.Security.Claims;
using System.Text;
using API.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace API.Features.Account.Services {
    public class TokenDescriptorProvider {

        private readonly JwtOptions _jwtOptions;
        private SecurityTokenDescriptor _descriptor;

        public TokenDescriptorProvider(IOptions<JwtOptions> jwtOptions) {
            _jwtOptions = jwtOptions?.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
            BuildDescriptor();
        }

        public SecurityTokenDescriptor Provide() {
            return _descriptor;
        }

        public void AddSigningCredentials() {
            if (_descriptor is null) {
                BuildDescriptor();
            }
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            _descriptor.SigningCredentials = new SigningCredentials(key, _jwtOptions.SigningAlg);
        }

        public void AddSubjectClaims(string userEmail, Guid userGuid) {
            if (_descriptor is null) {
                BuildDescriptor();
            }
            
            var subject = new ClaimsIdentity();
            subject.AddClaim(new Claim(JwtRegisteredClaimNames.Email, userEmail));
            subject.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, userGuid.ToString()));
            _descriptor.Subject = subject;
        }

        private void BuildDescriptor() {
            _descriptor = new SecurityTokenDescriptor {
                Audience = _jwtOptions.Audience,
                Expires = DateTime.UtcNow + TimeSpan.FromHours(_jwtOptions.TokenLifetimeInHours),
                Issuer = _jwtOptions.Issuer,
                NotBefore = DateTime.UtcNow,
                IssuedAt = DateTime.UtcNow,
                TokenType = "JWT"
            };
        }
    }
}