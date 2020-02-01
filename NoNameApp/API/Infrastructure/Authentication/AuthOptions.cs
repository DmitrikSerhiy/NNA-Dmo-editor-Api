﻿using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace API.Infrastructure.Authentication {
    public class AuthOptions {
        //todo: locate it to the appsettings.json later or some secure place
        public const string ISSUER = "NoNameAppApi";
        public const string AUDIENCE = "NoNameAppFrontA8";
        private const string KEY = "mysupersecret_secretkey!123";
        public const int LIFETIME = 10;
        //todo: add this policy later. Look into startup
        public static SymmetricSecurityKey GetSymmetricSecurityKey() {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}