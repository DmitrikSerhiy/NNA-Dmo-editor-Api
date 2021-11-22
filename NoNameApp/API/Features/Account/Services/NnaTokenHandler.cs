﻿using System.Linq;
using Microsoft.IdentityModel.JsonWebTokens;
using Model.Enums;

namespace API.Features.Account.Services {
    public sealed class NnaTokenHandler : JsonWebTokenHandler {
        public string GetTokenKeyId(string token) {
            if (!CanReadToken(token)) {
                return null;
            }
            
            return ReadJsonWebToken(token).Claims
                .FirstOrDefault(claim => claim.Type == nameof(NnaCustomTokenClaims.oid))?.Value;
        }
        
        public string GetUserEmail(string token) {
            if (!CanReadToken(token)) {
                return null;
            }
            
            return ReadJsonWebToken(token).Claims
                .FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Email)?.Value;
        }
    }
}