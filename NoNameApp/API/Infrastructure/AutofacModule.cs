using API.Infrastructure.Authentication;
using Autofac;
using Autofac.Features.ResolveAnything;
using Microsoft.Extensions.Logging;
using Model;
using Persistence;

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
                .RegisterAssemblyTypes(typeof(NoNameContext).Assembly)
                .Where(t => t.Name.EndsWith("Repository"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterSource(
                new AnyConcreteTypeNotAlreadyRegisteredSource()
                    .WithRegistrationsAs(b => b.InstancePerLifetimeScope()));
        }
    }
}
