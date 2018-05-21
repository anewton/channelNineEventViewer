using Autofac;

namespace ChannelNineEventFeed.WPF
{
    public class DependencyModule
    {
        public void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<Library.DependencyModule>();
            builder.RegisterModule<Data.DependencyModule>();
        }
    }
}
