using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Model.DTOs.Account;

namespace API.Features.Account.Services {
    public class IdentityService {
        private readonly NnaUserManager _userManager;
        public IdentityService(NnaUserManager userManager) {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<ClaimsIdentity> GetIdentity(string email, string password) {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return null;
            }

            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                return null;
            }

            var claimsIdentity = new ClaimsIdentity(
                new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName)
                },
                "Login",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }

        public string CreateJwt(ClaimsIdentity identity) {
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: AuthOptionsDto.ISSUER,
                audience: AuthOptionsDto.AUDIENCE,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptionsDto.LIFETIME)),
                signingCredentials: new SigningCredentials(AuthOptionsDto.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
