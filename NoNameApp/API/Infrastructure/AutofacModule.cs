using API.Controllers;
using Autofac;
using Autofac.Features.ResolveAnything;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Model;
using Persistence;

namespace API.Infrastructure {
    public class AutofacModule : Module {
        public AutofacModule() { }
        protected override void Load(ContainerBuilder builder) {

            builder
                .RegisterType<LoggerFactory>()
                .As<ILoggerFactory>()
                .UsingConstructor()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<UnitOfWork>()
                .InstancePerLifetimeScope();

            builder.RegisterSource(
                new AnyConcreteTypeNotAlreadyRegisteredSource()
                    .WithRegistrationsAs(b => b.InstancePerLifetimeScope()));
        }
    }
}
