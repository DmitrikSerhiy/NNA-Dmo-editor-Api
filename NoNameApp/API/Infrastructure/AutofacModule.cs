using API.Infrastructure.Authentication;
using API.Services;
using Autofac;
using Autofac.Features.ResolveAnything;
using Microsoft.Extensions.Logging;
using Model;
using Persistence;
using IEditorService = API.Services.IEditorService;

namespace API.Infrastructure
{
    public class AutofacModule : Module {

        protected override void Load(ContainerBuilder builder) {

            builder
                .RegisterType<LoggerFactory>()
                .As<ILoggerFactory>()
                .UsingConstructor()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<UnitOfWork>()
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
                .RegisterAssemblyTypes(typeof(NnaContext).Assembly)
                .Where(t => t.Name.EndsWith("Repository"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterSource(
                new AnyConcreteTypeNotAlreadyRegisteredSource()
                    .WithRegistrationsAs(b => b.InstancePerLifetimeScope()));
        }
    }
}
