using ChannelNineEventFeed.Data.Feeds;
using ChannelNineEventFeed.Data.Interfaces;
using ChannelNineEventFeed.Data.Interfaces.Repos;
using ChannelNineEventFeed.Library.Intefaces;
using ChannelNineEventFeed.Library.Interfaces;
using ChannelNineEventFeed.Library.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using System.Xml;

namespace ChannelNineEventFeed.Data.Services
{
    public class FeedService : IFeedService
    {
        private readonly IAppSettings _appSettings;
        private readonly IEventService _eventService;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMediaRepository _mediaRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly ISessionCategoryRepository _sessionCategoryRepository;
        private readonly ISpeakerRepository _speakerRepository;
        private readonly ISpeakerVideoRepository _speakerVideoRepository;
        private readonly IVideoRepository _videoRepository;

        public FeedService(IAppSettings appSettings, IEventService eventService, ICategoryRepository categoryRepository, IMediaRepository mediaRepository, ISessionRepository sessionRepository, ISessionCategoryRepository sessionCategoryRepository, ISpeakerRepository speakerRepository, ISpeakerVideoRepository speakerVideoRepository, IVideoRepository videoRepository)
        {
            _appSettings = appSettings;
            _eventService = eventService;
            _categoryRepository = categoryRepository;
            _mediaRepository = mediaRepository;
            _sessionRepository = sessionRepository;
            _sessionCategoryRepository = sessionCategoryRepository;
            _speakerRepository = speakerRepository;
            _speakerVideoRepository = speakerVideoRepository;
            _videoRepository = videoRepository;
        }

        public IEnumerable<IEvent> GetEvents()
        {
            return _eventService.GetEvents();
        }

        public void DownloadFilteredEventData(IEventFilters filters)
        {
            var events = GetEvents();
            if (filters.EventNames.Count() > 0)
            {
                foreach (var eventName in filters.EventNames)
                {
                    var eventMetaData = events.FirstOrDefault(x => x.Name == eventName);
                    if (filters.EventYears.Count() > 0)
                    {
                        foreach (var eventYear in eventMetaData.Years)
                        {
                            var yearFilter = filters.EventYears.FirstOrDefault(x => x == eventYear);
                            if (!string.IsNullOrEmpty(yearFilter))
                            {
                                // Check database for existing session data
                                var eventSessionDataCount = _sessionRepository.CountByEventNameAndYear(eventName, yearFilter);
                                if (eventSessionDataCount == 0)
                                {
                                    // Download session data for the event name and year
                                    eventSessionDataCount = DownloadAndSaveSessionData(eventName, yearFilter);
                                    if (eventSessionDataCount > 0)
                                    {
                                        foreach (var feed in VideoFeed.VideoFeeds)
                                        {
                                            feed.FeedYear = yearFilter;
                                            DownloadAndSaveVideoData(feed, eventName);
                                        }
                                        DownloadAndSaveSpeakerData(eventName, yearFilter);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private int DownloadAndSaveSessionData(string eventName, string year)
        {
            var countOfSessions = 0;
            var sessionFeed = new SessionFeed(eventName, year);
            using (var webClient = new System.Net.WebClient())
            {
                try
                {
                    webClient.Headers.Add(System.Net.HttpRequestHeader.ContentType, "application/json; charset=utf-8");
                    var sessionJson = webClient.DownloadString(sessionFeed.SessionFeedUrl);
                    var sessionList = JsonConvert.DeserializeObject<List<Session>>(sessionJson);
                    if (sessionList != null)
                    {
                        //de-dup
                        sessionList = sessionList.Distinct().ToList();
                        foreach (var session in sessionList)
                        {
                            session.EventYear = year;
                            session.EventName = eventName;
                            var foundSession = _sessionRepository.FindByTitleAndEventYear(session.Title, year);
                            if (foundSession == null)
                            {
                                var insertedSession = _sessionRepository.Add(session);
                                session.Id = insertedSession.Id;
                            }
                        }
                        countOfSessions = sessionList.Count;
                        foreach (var s in sessionList)
                        {
                            //Save tags to categories table
                            SaveSessionCategories(s.Id, s.Tags);

                            //Save all media
                            if (!string.IsNullOrEmpty(s.Wmvhq))
                            {
                                SaveMediaData("wmvhigh", s.Wmvhq, s.Id);
                            }
                            if (!string.IsNullOrEmpty(s.Wmv))
                            {
                                SaveMediaData("wmv", s.Wmv, s.Id);
                            }
                            if (!string.IsNullOrEmpty(s.Mp4high))
                            {
                                SaveMediaData("mp4high", s.Mp4high, s.Id);
                            }
                            if (!string.IsNullOrEmpty(s.Mp4med))
                            {
                                SaveMediaData("mp4med", s.Mp4med, s.Id);
                            }
                            if (!string.IsNullOrEmpty(s.Mp4low))
                            {
                                SaveMediaData("mp4", s.Mp4low, s.Id);
                            }
                        }
                    }
                }
                catch (System.Net.WebException webExc)
                {
                    Debug.WriteLine(webExc.ToString());
                }
                //catch (Exception)
                //{
                //    throw;
                //}
            }
            return countOfSessions;
        }

        private void SaveSessionCategories(int sessionId, IList<string> tags)
        {
            foreach (var tag in tags)
            {
                var category = _categoryRepository.FindByName(tag);
                if (category == null)
                {
                    category = _categoryRepository.Add(new Category { Name = tag });
                }
                var sessionCategory = _sessionCategoryRepository.FindBySessionIdAndCategoryId(sessionId, category.Id);
                if (sessionCategory == null)
                {
                    _sessionCategoryRepository.Add(new SessionCategory() { SessionId = sessionId, CategoryId = category.Id });
                }
            }
        }

        private void SaveMediaData(string mediaType, string downloadLink, int sessionId)
        {
            var media = _mediaRepository.FindBySessionIdAndType(sessionId, mediaType);
            if (media == null)
            {
                var newMedia = new Media()
                {
                    SessionId = sessionId,
                    MediaType = mediaType,
                    DownloadLink = downloadLink,
                    IsPlayableInMediaElement = true,
                    IsDownloaded = false,
                    IsDownloadInProgress = false
                };
                _mediaRepository.Add(newMedia);
            }
        }

        private void DownloadAndSaveVideoData(VideoFeed videoFeed, string eventName)
        {
            var hasMoreItems = true;
            int pageIndex = 1;
            string lastItemTitle = string.Empty;
            do
            {
                SyndicationFeed feed = null;
                try
                {
                    using (XmlReader reader = XmlReader.Create(videoFeed.GetBuildVideoFeedUrlByPage(pageIndex, eventName)))
                    {
                        feed = SyndicationFeed.Load(reader);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }

                if (feed != null && feed.Items != null && feed.Items.Count() > 0)
                {
                    //check last item title for match
                    if (lastItemTitle == feed.Items.Last().Title.Text)
                    {
                        break;
                    }
                    foreach (var item in feed.Items)
                    {
                        var title = CleanVideoTitle(item.Title.Text);
                        var video = _videoRepository.FindByTitle(title);
                        if (video == null)
                        {
                            video = SaveVideoData(item, videoFeed.MediaType, eventName, videoFeed.FeedYear);
                        }
                        else
                        {
                            UpdateVideoData(item, videoFeed.MediaType);
                        }

                        //Speakers
                        var creators = GetExtensionElementValue(item, "creator");
                        if (!string.IsNullOrEmpty(creators))
                        {
                            var comma = ",".ToCharArray();
                            var presenterList = creators.Split(comma, StringSplitOptions.RemoveEmptyEntries);
                            if (presenterList.Count() > 0)
                            {
                                foreach (var presenter in presenterList)
                                {
                                    var speaker = _speakerRepository.FindByNameAndEventYear(eventName, presenter.Trim(), videoFeed.FeedYear);
                                    if (speaker == null)
                                    {
                                        speaker = _speakerRepository.Add(new Speaker() { Name = presenter.Trim(), EventYear = videoFeed.FeedYear, EventName = eventName });
                                    }
                                    var speakerVideo = _speakerVideoRepository.FindBySpeakerIdAndVideoId(speaker.Id, video.Id);
                                    if (speakerVideo == null)
                                    {
                                        _speakerVideoRepository.Add(new SpeakerVideo() { SpeakerId = speaker.Id, VideoId = video.Id });
                                    }
                                }
                            }
                        }
                    }
                    lastItemTitle = feed.Items.Last().Title.Text;
                    pageIndex++;
                }
                else
                {
                    hasMoreItems = false;
                }
            }
            while (hasMoreItems == true);
        }

        private IVideo SaveVideoData(SyndicationItem item, string mediaType, string eventName, string year)
        {
            IVideo video = null;
            try
            {
                string sessionCode = string.Empty;
                string hostedPageLink = string.Empty;
                if (item.Links != null && item.Links.Count > 0)
                {
                    var link = item.Links.Where(l => l.RelationshipType != null && l.RelationshipType == "alternate").FirstOrDefault();
                    if (link != null && link.Uri != null)
                    {
                        hostedPageLink = link.Uri.AbsoluteUri;
                        sessionCode = link.Uri.Segments[link.Uri.Segments.Count() - 1].ToString();
                    }
                }
                var parsedDuration = int.TryParse(GetExtensionElementValue(item, "duration"), out int durationSeconds);
                var summary = HttpUtility.HtmlDecode(GetExtensionElementValue(item, "summary"));
                video = new Video()
                {
                    Title = CleanVideoTitle(item.Title.Text),
                    SummaryHtml = item.Summary.Text,
                    Summary = summary,
                    PublishDate = item.PublishDate.LocalDateTime,
                    HostedPageLink = hostedPageLink,
                    SessionCode = sessionCode,
                    DurationSeconds = durationSeconds,
                    IsVideoViewingComplete = false,
                    EventYear = year,
                    EventName = eventName
                };
                video = _videoRepository.Add(video);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return video;
        }

        private void UpdateVideoData(SyndicationItem item, string mediaType)
        {
            try
            {
                string sessionCode = string.Empty;
                string hostedPageLink = string.Empty;
                if (item.Links != null && item.Links.Count > 0)
                {
                    var link = item.Links.Where(l => l.RelationshipType != null && l.RelationshipType == "alternate").FirstOrDefault();
                    if (link != null && link.Uri != null)
                    {
                        hostedPageLink = link.Uri.AbsoluteUri;
                        sessionCode = link.Uri.Segments[link.Uri.Segments.Count() - 1].ToString();
                    }
                }
                var title = CleanVideoTitle(item.Title.Text);
                var video = _videoRepository.FindByTitle(title);
                if (video != null)
                {
                    if (!string.IsNullOrEmpty(hostedPageLink))
                    {
                        video.HostedPageLink = hostedPageLink;
                    }
                    if (!string.IsNullOrEmpty(sessionCode))
                    {
                        video.SessionCode = sessionCode;
                    }
                    _videoRepository.Update(video);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private string CleanVideoTitle(string videoTitle)
        {
            if (string.IsNullOrEmpty(videoTitle))
            {
                return string.Empty;
            }
            return videoTitle.Replace(':', ' ').Replace('"', ' ').Replace('/', '-').Replace('?', ' ');
        }

        private string GetExtensionElementValue(SyndicationItem item, string extensionElementName)
        {
            if (item == null || item.ElementExtensions.Count == 0)
            {
                return string.Empty;
            }
            var elementExtension = item.ElementExtensions.Where(x => x.OuterName == extensionElementName).FirstOrDefault();
            if (elementExtension == null)
            {
                return string.Empty;
            }
            try
            {
                return item.ElementExtensions.Where(x => x.OuterName == extensionElementName).First().GetObject<string>();
            }
            catch
            {
                return string.Empty;
            }
        }

        private void DownloadAndSaveSpeakerData(string eventName, string year)
        {
            var speakerFeed = new SpeakerFeed(eventName) { FeedYear = year };
            var hasMoreItems = true;
            var lastItemTitle = string.Empty;
            int pageIndex = 1;
            do
            {
                SyndicationFeed feed = null;
                try
                {
                    using (XmlReader reader = XmlReader.Create(speakerFeed.GetBuildSpeakerFeedUrlByPage(pageIndex)))
                    {
                        feed = SyndicationFeed.Load(reader);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
                if (feed != null && feed.Items != null && feed.Items.Count() > 0)
                {
                    //check last item title for match
                    if (lastItemTitle == feed.Items.Last().Title.Text)
                    {
                        break;
                    }
                    foreach (var item in feed.Items)
                    {
                        var speaker = _speakerRepository.FindByNameAndEventYear(eventName, item.Title.Text, year);
                        if (speaker == null)
                        {
                            SaveSpeakerData(item, eventName, year);
                        }
                        else
                        {
                            UpdateSpeakerData(speaker, item, eventName, year);
                        }
                    }
                    lastItemTitle = feed.Items.Last().Title.Text;
                    pageIndex++;
                }
                else
                {
                    hasMoreItems = false;
                }
            }
            while (hasMoreItems == true);
        }

        private void UpdateSpeakerData(ISpeaker speaker, SyndicationItem item, string eventName, string year)
        {
            try
            {
                var hostedPageLink = string.Empty;
                if (item.Links != null && item.Links.Count > 0)
                {
                    var link = item.Links.Where(l => l.RelationshipType != null && l.RelationshipType == "alternate").FirstOrDefault();
                    if (link != null && link.Uri != null)
                    {
                        hostedPageLink = link.Uri.AbsoluteUri;
                    }
                }
                speaker.Name = item.Title.Text;
                speaker.PublishDate = item.PublishDate.LocalDateTime;
                speaker.HostedPageLink = hostedPageLink;
                speaker.Summary = HttpUtility.HtmlDecode(GetExtensionElementValue(item, "summary"));
                speaker.EventYear = year;
                speaker.EventName = eventName;
                _speakerRepository.Update(speaker);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void SaveSpeakerData(SyndicationItem item, string eventName, string year)
        {
            try
            {
                var hostedPageLink = string.Empty;
                if (item.Links != null && item.Links.Count > 0)
                {
                    var link = item.Links.Where(l => l.RelationshipType != null && l.RelationshipType == "alternate").FirstOrDefault();
                    if (link != null && link.Uri != null)
                    {
                        hostedPageLink = link.Uri.AbsoluteUri;
                    }
                }
                var speaker = new Speaker()
                {
                    Name = item.Title.Text.Trim(),
                    PublishDate = item.PublishDate.LocalDateTime,
                    HostedPageLink = hostedPageLink,
                    Summary = HttpUtility.HtmlDecode(GetExtensionElementValue(item, "summary")),
                    EventYear = year,
                    EventName = eventName
                };
                _speakerRepository.Add(speaker);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}