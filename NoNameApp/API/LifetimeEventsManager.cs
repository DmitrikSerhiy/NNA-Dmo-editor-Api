using System;
using System.Threading;
using System.Threading.Tasks;
using API.Helpers.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Model.Interfaces.Repositories;

namespace API {
    internal class LifetimeEventsManager : IHostedService {
        private readonly IWebHostEnvironment _environment;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly IUserRepository _repository;

        public LifetimeEventsManager(
            IWebHostEnvironment environment,
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
            _repository.SanitiseEditorConnections();
            if (!_environment.IsLocalMachine()) {
                _repository.SanitiseUserTokens();
            }

            Console.WriteLine("Tokens and connections are sanitised");
        }
    }
}