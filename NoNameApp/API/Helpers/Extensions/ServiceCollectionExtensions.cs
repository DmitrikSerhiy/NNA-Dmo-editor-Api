using System;
using System.Text;
using System.Threading.Tasks;
using API.Features.Account.Services;
using API.Features.Account.Services.Local;
using API.Helpers.GlobalFilters;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Model.Entities;
using Persistence;
using Serilog;

namespace API.Helpers.Extensions {
    public static class ServiceCollectionExtensions {
        private static readonly string angularClientOrigin = "angularClient";
        
        public static void AddNnaDbOptions(this IServiceCollection services, IConfiguration configuration) {
            services.AddDbContext<NnaContext>(options => {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), sqlOptions => {
                    sqlOptions.EnableRetryOnFailure(1);
                    sqlOptions.MigrationsAssembly("Persistence");
                });
            });
        }
        
        public static void AddNnaCorsOptions(this IServiceCollection services, IConfiguration configuration) {
            services.AddCors(o => {
                o.AddPolicy(angularClientOrigin, policyBuilder => {
                    policyBuilder.WithOrigins(configuration["CorsUrls"].Split(","));
                    policyBuilder.AllowAnyMethod();
                    policyBuilder.AllowAnyHeader();
                    policyBuilder.AllowCredentials();
                });
            });
        }
        
        public static void UseNnaCorsOptions(this IApplicationBuilder app) {
            app.UseCors(angularClientOrigin);
        }
        
        public static void UseNnaAccountRewriteOptions(this IApplicationBuilder app) {
            var options = new RewriteOptions()
                .AddRewrite("api/account/register", "api/local/account/register", skipRemainingRules: false)
                .AddRewrite("api/account/token", "api/local/account/token", skipRemainingRules: false)
                .AddRewrite("api/account/name", "api/local/account/name", skipRemainingRules: false)
                .AddRewrite("api/account/email", "api/local/account/email", skipRemainingRules: false);
            app.UseRewriter(options);
        }

        
        public static void AddNnaLocalLoggerOptions(this IServiceCollection services, IWebHostEnvironment environment, IConfiguration configuration) {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
        
        
        public static void AddNnaMvcAndFilters(this IServiceCollection services) {
            services
                .AddControllersWithViews(options => {
                    options.Filters.Add(typeof(ExceptionFilter));
                    options.Filters.Add(typeof(TransactionFilter));
                })
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());
        }

        public static void AddNnaAuthenticationOptions(this IServiceCollection services) {
            var jwtOptions = services.BuildServiceProvider().GetService<IOptions<JwtOptions>>();
            var tokenDescriptorProvider = new TokenDescriptorProvider(jwtOptions);
            var tokenDescriptor = tokenDescriptorProvider.ProvideForAccessToken();
            tokenDescriptor.AddSigningCredentials(jwtOptions.Value);

            var identityBuilder = services
                .AddIdentity<NnaUser, NnaRole>(options => {
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequiredLength = 10;
                    options.Password.RequireDigit = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequiredUniqueChars = 5; // todo: cover with front-end logic
                    options.Password.RequireNonAlphanumeric = true;
                })
                .AddEntityFrameworkStores<NnaContext>();
            identityBuilder.AddUserManager<NnaUserManager>();


            services.AddAuthentication(options => {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = TokenValidationParametersProvider.Provide(tokenDescriptor);
                    
                    options.Events = new JwtBearerEvents {
                        OnMessageReceived = context => {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/api/editor"))) {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context => {
                            Console.WriteLine("Token is valid!!!");
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context => {
                            Console.WriteLine("Failed to validate token");
                            Console.WriteLine(context.Exception);
                            return Task.CompletedTask;
                        }
                    };
                });
        }
        
        
        public static void AddNnaLocalAuthenticationOptions(this IServiceCollection services, IConfiguration configuration) {
            
            var jwtOptions = new JwtOptions();
            configuration.GetSection(nameof(JwtOptions)).Bind(jwtOptions);
            
            var identityBuilder = services
                .AddIdentity<NnaUser, NnaRole>(options => {
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequiredLength = 3;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<NnaContext>();
            identityBuilder.AddUserManager<NnaLocalUserManager>();

            services.AddAuthentication(options => {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = false,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(jwtOptions.Key)),
                        ValidateIssuerSigningKey = true
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