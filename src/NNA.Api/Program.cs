using Microsoft.AspNetCore.Http.Connections;
using NNA.Api;
using NNA.Api.Extensions;
using NNA.Api.Features.Account.Services;
using NNA.Api.Features.Editor.Hubs;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsLocalMachine())
{
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateBootstrapLogger();
}

try
{
    builder.Services.AddControllers();
    builder.AddNnaAzureKeyVaultAndSecretsOptions();
    builder.AddNnaDbOptions();
    builder.AddNnaCorsOptions();
    builder.AddNnaOptions();
    builder.AddNnaAuthenticationOptions();

    builder.Services
        .AddSignalR()
        .AddMessagePackProtocol()
        .AddHubOptions<EditorHub>(o => { o.EnableDetailedErrors = true; });

    builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    builder.Services.AddNnaMvcAndFilters();
    builder.Services.AddHostedService<LifetimeEventsManager>();
    // todo: register validators, try with automatic validation
    builder.AddAutofacContainer();
    if (builder.Environment.IsLocalMachine())
    {
        builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));
    }

    var app = builder.Build();

    if (builder.Environment.IsLocalMachine())
    {
        app.UseSerilogRequestLogging();
        app.UseDeveloperExceptionPage();
    }
    app.UseHttpsRedirection();
    app.UseHsts();
    app.UseRouting();
    app.UseNnaCorsOptions();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseMiddleware<AuthenticatedIdentityMiddleware>();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
        endpoints.MapHub<EditorHub>("api/editor", options => { options.Transports = HttpTransportType.WebSockets; });
    });

    app.Run();
}
catch (Exception ex) {
    Log.Fatal(ex, "Unhandled startup exception");
    throw;
} finally {
    Log.Information("Shut down complete");
    Log.CloseAndFlush();  
}