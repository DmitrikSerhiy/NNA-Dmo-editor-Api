using API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase {
        private readonly NoNameUserManager _userManager;

        public AccountController(
            NoNameUserManager userManager) {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }


        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(String email, String userName, String password) {
            var notUniqueEmail = await _userManager.FindByEmailAsync(email);
            if (notUniqueEmail != null) {
                return BadRequest();
            }

            var notUniqueUserName = await _userManager.FindByNameAsync(userName);
            if (notUniqueUserName != null) {
                return BadRequest();
            }

            var result = await _userManager.CreateAsync(new NoNameUser(email, userName), password);
            if (!result.Succeeded) {
                return BadRequest();//todo: change it to server error or empty ok
            }

            var identity = await GetIdentity(email, password);
            var jwt = CreateJWT(identity);

            return new JsonResult(new {
                accessToken = jwt,
                userName = identity.Name,
                email
            });
        }

        [HttpPost]
        [Route("token")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(String email, String password) {
            if (String.IsNullOrWhiteSpace(email) || String.IsNullOrWhiteSpace(password)) {
                return BadRequest();
            }

            var identity = await GetIdentity(email, password);
            if (identity == null) {
                return BadRequest(new {errorText = "Invalid email or password."});
            }

            var jwt = CreateJWT(identity);

            return new JsonResult(new {
                accessToken = jwt,
                userName = identity.Name,
                email
            });
        }

        //TODO: relocate it to separate service
        private async Task<ClaimsIdentity> GetIdentity(String email, String password) {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) {
                return null;
            }

            if (!await _userManager.CheckPasswordAsync(user, password)) {
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

        private String CreateJWT(ClaimsIdentity identity) {
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        } 

    }
}
