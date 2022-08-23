using Azure.Core;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Model;
using Model.Entities;
using Model.Enums;
using Model.Models;
using NNA.Api.Features.Account.Services;
using NNA.Api.Filters;
using NNA.Api.Helpers;
using Persistence;
using Serilog;

namespace NNA.Api.Extensions; 

public static class ApiBuilder {
    private static readonly string angularClientOrigin = "angularClient";

    public static bool IsLocal(this IHostEnvironment env) {
        return env.EnvironmentName == "Local";
    }
    
    public static bool IsLocalMachine(this IHostEnvironment env) {
        return Environment.GetEnvironmentVariable("LocalMachine") == "true";
    }
    
    public static void AddNnaLocalLoggerOptions(this WebApplicationBuilder builder) {
        builder.Host.UseSerilog();
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();
    }
    
    public static void UseNnaCorsOptions(this IApplicationBuilder app) {
        app.UseCors(angularClientOrigin);
    }
    
    public static void AddNnaCorsOptions(this WebApplicationBuilder builder) {
        builder.Services.AddCors(o => {
            o.AddPolicy(angularClientOrigin, policyBuilder => {
                policyBuilder.WithOrigins(builder.Configuration["CorsUrls"].Split(","));
                policyBuilder.WithExposedHeaders(
                    nameof(NnaHeaderNames.ExpiredToken), 
                    nameof(NnaHeaderNames.RedirectToLogin));
                policyBuilder.AllowAnyMethod();
                policyBuilder.AllowAnyHeader();
                policyBuilder.AllowCredentials();
            });
        });
    }
    
    public static void AddNnaMvcAndFilters(this IServiceCollection services) {
        services
            .AddControllersWithViews(options => {
                options.Filters.Add(typeof(ValidationFilter));
                options.Filters.Add(typeof(ExceptionFilter));
                options.Filters.Add(typeof(TransactionFilter));
            })
            .ConfigureApiBehaviorOptions(options => {
                options.SuppressModelStateInvalidFilter = true;
            });
    }


    public static void AddNnaAuthenticationOptions(this WebApplicationBuilder builder) {
        var jwtOptions = builder.Services.BuildServiceProvider().GetService<IOptions<JwtOptions>>();
        var tokenDescriptorProvider = new TokenDescriptorProvider(jwtOptions!.Value);
        var tokenDescriptor = tokenDescriptorProvider.ProvideForAccessToken();
        tokenDescriptor.AddSigningCredentials(jwtOptions.Value);

        var identityBuilder = builder.Services
            .AddIdentity<NnaUser, NnaRole>(options => {
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = new UserOptions().AllowedUserNameCharacters += " ";
                options.Password.RequiredLength = ApplicationConstants.MinPasswordLength;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredUniqueChars = ApplicationConstants.MinPasswordLength / 2;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<NnaContext>();
        identityBuilder.AddUserManager<NnaUserManager>();
        identityBuilder.AddTokenProvider<DataProtectorTokenProvider<NnaUser>>("");

        builder.Services
            .AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => {
                options.TokenValidationParameters = TokenValidationParametersProvider.Provide(tokenDescriptor);

                options.Events = new JwtBearerEvents {
                    OnMessageReceived = context => {
                        var accessToken = context.Request.Query["access_token"];
                        if (!string.IsNullOrEmpty(accessToken) &&
                            context.HttpContext.Request.Path.StartsWithSegments("/api/editor")) {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context => {
                        // expiration error code toke from Microsoft.IdentityModel.Tokens.LogMessages
                        if (context.Exception.Message.StartsWith("IDX10223")) {
                            context.Response.Headers.Add(NnaHeaders.Get(NnaHeaderNames.ExpiredToken));
                        }

                        return Task.CompletedTask;
                    }
                };
            });
    }

    public static void AddNnaOptions(this WebApplicationBuilder builder) {
        builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));
        builder.Services.Configure<SendGridConfiguration>(builder.Configuration.GetSection(nameof(SendGridConfiguration)));
    }

    public static void AddNnaDbOptions(this WebApplicationBuilder builder) {
        builder.Services.AddDbContextPool<NnaContext>(options => {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlOptions => {
                sqlOptions.EnableRetryOnFailure(1);
                sqlOptions.MigrationsAssembly("Persistence");
            });
        });
    }

    public static void AddNnaAzureKeyVaultAndSecretsOptions(this WebApplicationBuilder builder) {
        if (builder.Environment.IsLocalMachine() && !builder.Environment.IsLocal()) {
            builder.Configuration.AddNnaAzureKeyVault(new AzureCliCredential()); // for local run with develop env
        }
        else if (!builder.Environment.IsLocalMachine()) {
            builder.Configuration.AddNnaAzureKeyVault(new DefaultAzureCredential()); // for develop run with develop env
        }

        if (builder.Environment.IsLocal()) {
            builder.Configuration.AddUserSecrets("9287554d-fee5-4c75-8bd9-0ecbd051c423");
        }
    }
    
    private static void AddNnaAzureKeyVault(this IConfigurationBuilder configBuilder, TokenCredential credentials) {
        var configuration = configBuilder.Build();
        var secretClient = new SecretClient(
            new Uri($"https://{configuration["az-key-vault"]}.vault.azure.net/"),
            credentials);
        
        configBuilder.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
    }
}