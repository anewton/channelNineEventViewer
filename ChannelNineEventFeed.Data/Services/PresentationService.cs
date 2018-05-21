using ChannelNineEventFeed.Data.Interfaces;
using ChannelNineEventFeed.Data.Interfaces.Repos;
using ChannelNineEventFeed.Library.Interfaces;
using ChannelNineEventFeed.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChannelNineEventFeed.Data.Services
{
    public class PresentationService : IPresentationService
    {
        private readonly IFeedService _feedService;
        private readonly ISessionRepository _sessionRepository;
        private readonly ISessionCategoryRepository _sessionCategoryRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISpeakerRepository _speakerRepository;
        private readonly ISpeakerVideoRepository _speakerVideoRepository;
        private readonly IMediaRepository _mediaRepository;
        private readonly IVideoRepository _videoRepository;

        public PresentationService(IFeedService feedService, ISessionRepository sessionRepository, ISessionCategoryRepository sessionCategoryRepository, ICategoryRepository categoryRepository, ISpeakerRepository speakerRepository, ISpeakerVideoRepository speakerVideoRepository, IMediaRepository mediaRepository, IVideoRepository videoRepository)
        {
            _feedService = feedService;
            _sessionRepository = sessionRepository;
            _sessionCategoryRepository = sessionCategoryRepository;
            _categoryRepository = categoryRepository;
            _speakerRepository = speakerRepository;
            _speakerVideoRepository = speakerVideoRepository;
            _mediaRepository = mediaRepository;
            _videoRepository = videoRepository;
        }

        public IEnumerable<ICategory> GetFilteredCategories(IEventFilters filters)
        {
            IEnumerable<ICategory> result = null;
            // Remove any category filters first
            filters.CategorIds = new List<int>();
            var filteredSessionIds = _sessionRepository.FilterSessions(filters);
            if (filteredSessionIds != null)
            {
                var sessionCategories = _sessionCategoryRepository.FindBySessionId(filteredSessionIds);
                if (sessionCategories != null)
                {
                    var categoryIdList = sessionCategories.Select(x => x.CategoryId).Distinct().ToList();
                    result = _categoryRepository.GetCategoriesInSessionCategoryIdList(categoryIdList);
                }
            }
            return result.OrderBy(x => x.Name).ToList();
        }

        public IEnumerable<IPresentation> GetFilteredPresentations(IEventFilters filters)
        {
            // Check if filtered event and event years are already stored and if not download all event data
            _feedService.DownloadFilteredEventData(filters);

            // Get a list of Presentation objects
            List<IPresentation> presentations = null;

            // Get filtered Session ids
            var filteredSessionIds = _sessionRepository.FilterSessions(filters);
            if (filteredSessionIds != null)
            {
                var sessions = _sessionRepository.GetSessionsInSessionIdList(filteredSessionIds);

                // Filter for selected speakers
                if (filters.SpeakerIds != null && filters.SpeakerIds.Count() > 0)
                {
                    var videoIds = _speakerVideoRepository.GetVideoIdListWhereInSpeakerIdList(filters.SpeakerIds);
                    var videos = _videoRepository.GetVideosByVideoIdList(videoIds);
                    var videoMetaData = videos.Select(x => new { x.EventName, x.EventYear, x.SessionCode }).Distinct().ToList();

                    var filteredSessions = new List<ISession>();
                    foreach (var videoData in videoMetaData)
                    {
                        var sessionToAdd = sessions.Where(x => x.EventName == videoData.EventName && x.EventYear == videoData.EventYear && x.Code == videoData.SessionCode).FirstOrDefault();
                        if (sessionToAdd != null)
                        {
                            filteredSessions.Add(sessionToAdd);
                        }
                    }
                    sessions = filteredSessions;
                }

                if (sessions != null)
                {
                    var converter = new Converter<ISession, IPresentation>(SessionToPresentation);
                    presentations = sessions.ToList().ConvertAll(converter);
                    foreach (var presentation in presentations)
                    {
                        GetExtendedPresentationData(presentation);
                    }
                }
            }
            return presentations;
        }

        public IEnumerable<string> GetFilteredSessionTypes(IEventFilters filters)
        {
            List<string> result = null;
            // Remove any session type filters first
            filters.SessionTypes = new List<string>();
            var filteredSessionIds = _sessionRepository.FilterSessions(filters);
            if (filteredSessionIds != null)
            {
                var filteredSessions = _sessionRepository.GetSessionsInSessionIdList(filteredSessionIds);
                if (filteredSessions != null)
                {
                    result = filteredSessions.Select(x => x.SessionType).Distinct().OrderBy(x => x).ToList();
                }
            }
            return result;
        }

        public IEnumerable<ISpeaker> GetFilteredSpeakers(IEventFilters filters)
        {
            List<ISpeaker> result = null;
            // Remove any speaker filters first
            filters.SpeakerIds = new List<int>();
            var filteredSessionIds = _sessionRepository.FilterSessions(filters);
            if (filteredSessionIds != null)
            {
                var filteredSessions = _sessionRepository.GetSessionsInSessionIdList(filteredSessionIds);
                if (filteredSessions != null)
                {
                    var videoIds = _videoRepository.GetVideoIdListBySessionList(filteredSessions);
                    if (videoIds != null && videoIds.Count() > 0)
                    {
                        var speakerIds = _speakerVideoRepository.GetSpeakerIdListWhereInVideoIdList(videoIds);
                        if (speakerIds != null && speakerIds.Count() > 0)
                        {
                            result = _speakerRepository.GetWhereInSpeakerIdList(speakerIds).OrderBy(x => x.Name).ToList();
                        }
                    }
                }
            }
            return result;
        }

        public IPresentation GetPresentationBySessionId(int sessionId)
        {
            var session = _sessionRepository.FindById(sessionId);
            var presentation = SessionToPresentation(session);
            GetExtendedPresentationData(presentation);
            return presentation;
        }

        public ISession GetSessionBySessionId(int sessionId)
        {
            var session = _sessionRepository.FindById(sessionId);
            return session;
        }

        public IPresentation SessionToPresentation(ISession session)
        {
            return new Presentation()
            {
                Id = session.Id,
                Code = session.Code,
                Description = session.Description,
                EventName = session.EventName,
                EventYear = session.EventYear,
                Finish = !string.IsNullOrEmpty(session.Finish) ? DateTime.Parse(session.Finish) : DateTime.MinValue,
                Level = session.Level,
                Link = session.Link,
                SessionType = session.SessionType == null ? string.Empty : session.SessionType,
                SlidesLink = session.Slides,
                Starts = !string.IsNullOrEmpty(session.Starts) ? DateTime.Parse(session.Starts) : DateTime.MinValue,
                Thumbnailimage = session.Thumbnailimage == null ? string.Empty : session.Thumbnailimage.ToString(),
                Title = session.Title
            };
        }

        private IPresentation GetExtendedPresentationData(IPresentation presentation)
        {
            //Categories
            var sessionCategories = _sessionCategoryRepository.FindBySessionId(new List<int>() { presentation.Id });
            if (sessionCategories != null && sessionCategories.Count() > 0)
            {
                var sessionCategoryIds = sessionCategories.Select(sc => sc.CategoryId).Distinct().ToList();
                if (sessionCategoryIds != null && sessionCategoryIds.Count() > 0)
                {
                    presentation.Categories = _categoryRepository.GetCategoriesInSessionCategoryIdList(sessionCategoryIds);
                }
            }

            //Speakers
            var videoIds = _videoRepository.GetVideoIdListBySessionCode(presentation.Code);
            if (videoIds != null && videoIds.Count() > 0)
            {
                var speakerIds = _speakerVideoRepository.GetSpeakerIdListWhereInVideoIdList(videoIds);
                if (speakerIds != null && speakerIds.Count() > 0)
                {
                    presentation.Speakers = _speakerRepository.GetWhereInSpeakerIdList(speakerIds);
                }
            }

            //Media
            presentation.Media = _mediaRepository.FindBySessionId(presentation.Id);

            return presentation;
        }
    }
}