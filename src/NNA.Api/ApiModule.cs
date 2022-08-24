using Autofac;
using NNA.Api.Features.Account.Services;
using NNA.Api.Features.Editor.Services;
using NNA.Api.Helpers;
using NNA.Domain.Interfaces;

namespace NNA.Api;
public class ApiModule : Module
{
    protected override void Load(ContainerBuilder builder) {

        builder 
            .RegisterType<ClaimsValidator>()
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
    }
}

