using Microsoft.AspNetCore.Http.Connections;
using NNA.Api;
using NNA.Api.Extensions;
using NNA.Api.Features.Account.Services;
using NNA.Api.Features.Editor.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.AddNnaAzureKeyVaultAndSecretsOptions();
builder.AddNnaDbOptions();
builder.AddNnaCorsOptions();
builder.AddNnaOptions();
builder.AddNnaAuthenticationOptions();

builder.Services
    .AddSignalR()
    .AddMessagePackProtocol()
    .AddHubOptions<EditorHub>(o =>
    {
        o.EnableDetailedErrors = true;
    });
            
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddNnaMvcAndFilters();
builder.Services.AddHostedService<LifetimeEventsManager>();
// todo: register validators, try with automatic validation
builder.AddAutofacContainer();
var app = builder.Build();


app.UseDeveloperExceptionPage(); // todo: use only for non-prod env. when global errors handling strategy is ready.
app.UseHttpsRedirection();
app.UseHsts();
app.UseRouting();
app.UseNnaCorsOptions();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<AuthenticatedIdentityMiddleware>();

app.UseEndpoints(endpoints => {
    endpoints.MapControllers();
    endpoints.MapHub<EditorHub>("api/editor", options => {
        options.Transports = HttpTransportType.WebSockets;
    });
});

app.Run();