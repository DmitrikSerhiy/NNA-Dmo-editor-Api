﻿using Autofac;
using NNA.Domain.Interfaces;

namespace NNA.Persistence;

public sealed class PersistenceModule : Module {
    protected override void Load(ContainerBuilder builder) {
        builder
            .RegisterAssemblyTypes(typeof(NnaContext).Assembly)
            .Where(t => t.Name.EndsWith("Repository"))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        builder
            .RegisterType<ContextOrchestrator>()
            .As<IContextOrchestrator>()
            .InstancePerLifetimeScope();
    }
}