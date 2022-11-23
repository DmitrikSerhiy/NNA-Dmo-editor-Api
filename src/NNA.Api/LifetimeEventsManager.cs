using NNA.Api.Extensions;
using NNA.Domain.Interfaces.Repositories;
using Serilog;

namespace NNA.Api;

internal sealed class LifetimeEventsManager : IHostedService {
    private readonly IHostEnvironment _environment;
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly IUserRepository _repository;

    public LifetimeEventsManager(
        IHostEnvironment environment,
        IHostApplicationLifetime appLifetime,
        IUserRepository repository) {
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        _appLifetime = appLifetime ?? throw new ArgumentNullException(nameof(appLifetime));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public Task StartAsync(CancellationToken cancellationToken) {
        _appLifetime.ApplicationStopping.Register(OnStopping);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    private void OnStopping() {
        if (!_environment.IsLocalMachine()) {
            _repository.SanitiseEditorConnections();
            Log.Information("Editor connections are sanitised");
            
            _repository.SanitiseUserTokens();
            Log.Information("Tokens are sanitised");
        }
    }
}