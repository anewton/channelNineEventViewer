using Autofac;
using ChannelNineEventFeed.Data.Interfaces;
using ChannelNineEventFeed.Data.Interfaces.Repos;
using ChannelNineEventFeed.Data.Services;

namespace ChannelNineEventFeed.Data
{
    public class DependencyModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Database
            builder.RegisterType<Sqlite.Database>().As<IDatabase>();
            builder.RegisterType<Sqlite.DatabaseInitializer>().As<IDatabaseInitializer>();

            // Repositories
            builder.RegisterType<Sqlite.Repos.CategoryRepository>().As<ICategoryRepository>();
            builder.RegisterType<Sqlite.Repos.MediaRepository>().As<IMediaRepository>();
            builder.RegisterType<Sqlite.Repos.SessionRepository>().As<ISessionRepository>();
            builder.RegisterType<Sqlite.Repos.SessionCategoryRepository>().As<ISessionCategoryRepository>();
            builder.RegisterType<Sqlite.Repos.SpeakerRepository>().As<ISpeakerRepository>();
            builder.RegisterType<Sqlite.Repos.SpeakerVideoRepository>().As<ISpeakerVideoRepository>();
            builder.RegisterType<Sqlite.Repos.VideoRepository>().As<IVideoRepository>();
            builder.RegisterType<Sqlite.Repos.QueuedPresentationRepository>().As<IQueuedPresentationRepository>();

            // Services
            builder.RegisterType<EventService>().As<IEventService>();
            builder.RegisterType<FeedService>().As<IFeedService>();
            builder.RegisterType<PresentationService>().As<IPresentationService>();
            builder.RegisterType<MediaService>().As<IMediaService>();

            base.Load(builder);
        }
    }
}
