using API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers {
    //[Authorize]
    [ApiController]
    public class AccountController : ControllerBase {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(
            UserManager<ApplicationUser> userManager) {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [HttpGet("/test")]
        public IActionResult Test() {
            return Ok("Hello world!");
        }

        [HttpPost("/register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(String email, String password) {
            var existedUser = await _userManager.FindByEmailAsync(email);
            if (existedUser == null) {
                return BadRequest();
            }

            var result = await _userManager.CreateAsync(new ApplicationUser(email), password);
            if (!result.Succeeded) {
                return BadRequest();//todo: change it to server error or empty ok
            }

            return RedirectToAction("Token", new {email, password});
        }

        [HttpPost("/token")]
        [AllowAnonymous]
        public async Task<IActionResult> Token(String email, String password) {
            var identity = await GetIdentity(email, password);
            if (identity == null) {
                return BadRequest(new {errorText = "Invalid email or password."});
            }

            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256));
            
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new JsonResult(new {
                access_token = encodedJwt,
                username = identity.Name
            });
        }

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
                "Token",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }
    
    }
}
