using Autofac;
using FluentValidation;
using NNA.Api.Features.Account.Services;
using NNA.Api.Features.Characters.Services;
using NNA.Api.Features.Editor.Services;
using NNA.Api.Helpers;
using NNA.Domain.Interfaces;

namespace NNA.Api;

public sealed class ApiModule : Module {
    protected override void Load(ContainerBuilder builder) {
        builder
            .RegisterType<NnaTokenHandler>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder
            .RegisterType<NnaTokenManager>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder
            .RegisterType<ClaimsValidator>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder
            .RegisterType<MailService>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder
            .RegisterType<AuthenticatedIdentityProvider>()
            .As<IAuthenticatedIdentityProvider>()
            .InstancePerLifetimeScope();

        builder
            .RegisterType<EditorService>()
            .As<IEditorService>()
            .InstancePerLifetimeScope();
        
        builder
            .RegisterType<TempIdSanitizer>()
            .AsSelf()
            .InstancePerLifetimeScope();
        

        builder
            .RegisterAssemblyTypes(typeof(Program).Assembly)
            .Where(type =>
                type.Name.EndsWith("Validator") && type.GetInterfaces().Any(x => x.Name == nameof(IValidator)))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
    }
}