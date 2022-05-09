using API.Features.Account.Services;
using API.Features.Editor.Services;
using Autofac;
using Microsoft.Extensions.Logging;
using Model.Interfaces;

namespace API
{
    public class ApiModule : Module
    {
        protected override void Load(ContainerBuilder builder) {

            builder
                .RegisterType<LoggerFactory>()
                .As<ILoggerFactory>()
                .UsingConstructor()
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
}
