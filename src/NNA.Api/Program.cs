using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using NNA.Api;
using NNA.Api.Extensions;
using NNA.Api.Features.Account.Services;
using NNA.Api.Features.Editor.Hubs;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsLocalMachine()) {
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateBootstrapLogger();
}

try {
    builder.Services
        .AddControllers(options => { options.ModelValidatorProviders.Clear(); })
        .AddNewtonsoftJson();
    builder.Services.Configure<ApiBehaviorOptions>(options => {
        options.SuppressModelStateInvalidFilter = true;
        options.SuppressMapClientErrors = true;
    });
    builder.AddNnaAzureKeyVaultAndSecretsOptions();
    builder.AddNnaDbOptions();
    builder.AddNnaCorsOptions();
    builder.AddNnaOptions();
    builder.AddNnaAuthenticationOptions();
    builder.AddNnaRateLimiter();

    builder.Services
        .AddSignalR()
        .AddMessagePackProtocol()
        .AddHubOptions<EditorHub>(o => { o.EnableDetailedErrors = true; });

    builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    builder.Services.AddNnaMvcAndFilters();
    builder.Services.AddHostedService<LifetimeEventsManager>();
    builder.AddAutofacContainer();
    builder.AddNnaLogging();

    var app = builder.Build();

    if (builder.Environment.IsLocalMachine()) {
        app.UseSerilogRequestLogging();
    }

    app.UseHttpsRedirection();
    app.UseHsts();
    app.UseRouting();
    app.UseIpRateLimiting();
    app.UseNnaCorsOptions();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseMiddleware<NnaAuthenticationMiddleware>();

    app.UseEndpoints(endpoints => {
        endpoints.MapControllers();
        endpoints.MapHub<EditorHub>("api/editor", options => { options.Transports = HttpTransportType.WebSockets; });
    });

    app.Run();
}
catch (Exception ex) {
    Log.Fatal(ex, "Unhandled startup exception");
    throw;
}
finally {
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}