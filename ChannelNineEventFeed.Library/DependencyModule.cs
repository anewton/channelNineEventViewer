using Autofac;
using ChannelNineEventFeed.Library.Intefaces;
using ChannelNineEventFeed.Library.Shared;

namespace ChannelNineEventFeed.Library
{
    public class DependencyModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AppSettings>().As<IAppSettings>();

            base.Load(builder);
        }
    }
}
