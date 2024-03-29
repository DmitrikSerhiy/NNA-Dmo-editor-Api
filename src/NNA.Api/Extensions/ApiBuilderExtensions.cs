﻿using AspNetCoreRateLimit;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Azure.Core;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NNA.Api.Features.Account.Services;
using NNA.Api.Filters;
using NNA.Api.Helpers;
using NNA.Domain;
using NNA.Domain.Entities;
using NNA.Domain.Enums;
using NNA.Domain.Models;
using NNA.Persistence;
using Serilog;

namespace NNA.Api.Extensions;

public static class ApiBuilderExtensions {
    private static readonly string angularClientOrigin = "angularClient";

    public static bool IsLocal(this IHostEnvironment env) {
        return env.EnvironmentName == "Local";
    }

    public static bool IsLocalMachine(this IHostEnvironment env) {
        return Environment.GetEnvironmentVariable("LocalMachine") == "true";
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
        services.AddControllersWithViews(options => {
            options.Filters.Add(typeof(ValidationFilter));
            options.Filters.Add(typeof(ExceptionFilter));
            options.Filters.Add(typeof(TransactionFilter));
        });
    }


    public static void AddNnaAuthenticationOptions(this WebApplicationBuilder builder) {
        var jwtOptions = builder.Services.BuildServiceProvider().GetService<IOptions<JwtOptions>>();
        var tokenDescriptorProvider = new TokenDescriptorProvider(jwtOptions!.Value);
        var tokenDescriptor = tokenDescriptorProvider.ProvideForAccessToken();
        tokenDescriptor.AddSigningCredentials(jwtOptions.Value);

        builder.Services
            .AddIdentity<NnaUser, IdentityRole<Guid>>(options => {
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = new UserOptions().AllowedUserNameCharacters += " ";
                options.Password.RequiredLength = ApplicationConstants.MinPasswordLength;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredUniqueChars = ApplicationConstants.MinPasswordLength / 2;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<NnaContext>()
            .AddUserManager<NnaUserManager>()
            .AddRoleManager<NnaRoleManager>()
            .AddDefaultTokenProviders();

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
        
        // builder.Services.AddAuthorization();
        builder.Services.AddAuthorization(options => {

            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireClaim(Enum.GetName(NnaCustomTokenClaims.rls)!, Enum.GetNames<NnaRoles>())
                .Build();

            options.AddPolicy(ApplicationConstants.NotActiveUserPolicy, policy => policy
                .RequireAuthenticatedUser()
                .RequireClaim(Enum.GetName(NnaCustomTokenClaims.rls)!, Enum.GetName(NnaRoles.NotActiveUser)!, Enum.GetName(NnaRoles.ActiveUser)!, Enum.GetName(NnaRoles.SuperUser)!));

            options.AddPolicy(ApplicationConstants.ActiveUserPolicy, policy => policy
                .RequireAuthenticatedUser()
                .RequireClaim(Enum.GetName(NnaCustomTokenClaims.rls)!, Enum.GetName(NnaRoles.ActiveUser)!, Enum.GetName(NnaRoles.SuperUser)!));

            options.AddPolicy(ApplicationConstants.SuperUserPolicy, policy => policy
                .RequireAuthenticatedUser()
                .RequireClaim(Enum.GetName(NnaCustomTokenClaims.rls)!, Enum.GetName(NnaRoles.SuperUser)!));
        });
        
        builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, NnaAuthorizationMiddleware>();

    }

    public static void AddNnaRateLimiter(this WebApplicationBuilder builder) {
        builder.Services.AddMemoryCache();
        builder.Services.AddInMemoryRateLimiting();
        builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
    }
    
    public static void AddNnaOptions(this WebApplicationBuilder builder) {
        builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));
        builder.Services.Configure<SendGridConfiguration>(builder.Configuration.GetSection(nameof(SendGridConfiguration)));
        builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
        builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
        
    }

    public static void AddNnaDbOptions(this WebApplicationBuilder builder) {
        builder.Services.AddDbContextPool<NnaContext>(options => {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlOptions => {
                sqlOptions.EnableRetryOnFailure(2);
            });
        });
    }

    public static void AddNnaLogging(this WebApplicationBuilder builder) {
        if (builder.Environment.IsLocal()) { // for local env. log only in console and file with information log level
            builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));
        }
        else {
            builder.AddNnaAppInsightLogging(); // for dev. env. log into appInsight and console with warning log level 
            builder.Host.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
                .WriteTo.ApplicationInsights(services.GetRequiredService<TelemetryConfiguration>(), TelemetryConverter.Traces) 
                .ReadFrom.Configuration(context.Configuration));
        }
    }

    public static void AddNnaAppInsightLogging(this WebApplicationBuilder builder) {

        var options = new ApplicationInsightsServiceOptions()
        {
            ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"],
            EnableAuthenticationTrackingJavaScript = false,
            EnableQuickPulseMetricStream = false, // for live metrics with latency of 1 second. NO NEED FOR NOW.
            EnableEventCounterCollectionModule = false, // no counters are used by default by module is enables by default. NO NEED FOR NOW. But in future it might be used for collection GC execution (for example). More info here: https://learn.microsoft.com/en-us/dotnet/core/diagnostics/event-counters#available-counters
            DeveloperMode = false, // might slow down the app
            EnableActiveTelemetryConfigurationSetup = false, // should be false for new apps
            EnableDependencyTrackingTelemetryModule = false, // tracks local or remote http cals, db cals etc. NO NEED FOR NOW. Serilog sink is used instead.
            EnableDiagnosticsTelemetryModule = false, // these last four are used for custom metrics with information about the runtime.
            EnableAppServicesHeartbeatTelemetryModule = false, 
            EnableAzureInstanceMetadataTelemetryModule = false,
            EnableHeartbeat = false,

            EnableRequestTrackingTelemetryModule = true, // track all requests
            EnablePerformanceCounterCollectionModule = true, // track performance
            EnableAdaptiveSampling = true, // automatically reduce amount of metrics when app is under high load
            AddAutoCollectedMetricExtractor = true, // improve performance of searching in Azure Portal AppInsight. More info: https://learn.microsoft.com/en-us/azure/azure-monitor/app/pre-aggregated-metrics-log-metrics
        };

        builder.Services.AddApplicationInsightsTelemetry(options);
        builder.Services.AddSingleton<ITelemetryInitializer, NnaUserTelemetryInitializer>();
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

    public static void AddAutofacContainer(this WebApplicationBuilder builder) {
        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        builder.Host.ConfigureContainer<ContainerBuilder>(afcBuilder => {
            afcBuilder.RegisterModule(new PersistenceModule());
            afcBuilder.RegisterModule(new ApiModule());
        });
    }

    private static void AddNnaAzureKeyVault(this IConfigurationBuilder configBuilder, TokenCredential credentials) {
        var configuration = configBuilder.Build();
        var secretClient = new SecretClient(
            new Uri($"https://{configuration["az-key-vault"]}.vault.azure.net/"),
            credentials);

        configBuilder.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
    }
}