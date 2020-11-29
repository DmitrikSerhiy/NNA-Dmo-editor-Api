using System.Threading.Tasks;
using API.Features.Account.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Model.DTOs.Account;
using Model.Entities;
using Persistence;

namespace API.Helpers.Extensions {
    public static class AuthenticationOptions {
        public static void AddAuthenticationOptions(this IServiceCollection services) {
            var identityBuilder = services
                .AddIdentity<NnaUser, NnaRole>(options => {
                    // todo: add some password restrictions later
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequiredLength = 3;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<NnaContext>();
            //todo: add token provider for future Maybe I should use IdentityServer4 here
            identityBuilder.AddUserManager<NnaUserManager>();

            services.AddAuthentication(options => {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options => {
                    options.RequireHttpsMetadata = false; //todo: swap to true
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = AuthOptionsDto.ISSUER,
                        ValidateAudience = true,
                        ValidAudience = AuthOptionsDto.AUDIENCE,
                        ValidateLifetime = false,
                        //todo: change it later
                        IssuerSigningKey = AuthOptionsDto.GetSymmetricSecurityKey(),
                        ValidateIssuerSigningKey = true,
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;

                            if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/api/editor")))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });
        }
    }
}
