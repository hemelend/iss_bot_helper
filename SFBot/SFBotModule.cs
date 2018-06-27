using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using SFBot.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Web;
using SFBot.Models;

namespace SFBot
{
    public class SFBotModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<SFBotDialogFactory>()
                .Keyed<ISFBotDialogFactory>(FiberModule.Key_DoNotSerialize)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<RootDialog>()
                .As<IDialog<object>>()
                .InstancePerDependency();

            builder.RegisterType<AreaRegionsDialog>()
                .InstancePerDependency();

            builder.RegisterType<InMemoryAreaRegionsRepository>()
                .Keyed<IRepository<AreaRegion>>(FiberModule.Key_DoNotSerialize)
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterType<SubsidiariesDialog>()
                .InstancePerDependency();

            builder.RegisterType<InMemorySubsidiariesRepository>()
                .Keyed<IRepository<Subsidiary>>(FiberModule.Key_DoNotSerialize)
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
