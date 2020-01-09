using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Helpers {
    public class AuthOptions {
        //todo: locate it to the appsettings.json later
        public const string ISSUER = "MyAuthServer";
        public const string AUDIENCE = "MyAuthClient";
        const string KEY = "mysupersecret_secretkey!123";
        //public const int LIFETIME = 10;
        //todo: add this policy later. Look into startup
        public static SymmetricSecurityKey GetSymmetricSecurityKey() {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
