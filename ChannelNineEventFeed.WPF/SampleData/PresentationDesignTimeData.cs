using ChannelNineEventFeed.Library.Interfaces;
using ChannelNineEventFeed.Library.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace ChannelNineEventFeed.WPF.SampleData
{
    public class PresentationDesignTimeData : Presentation
    {
        public PresentationDesignTimeData()
        {
            Id = 4443;
            Title = "Angle Brackets, Curly Braces, One ASP.NET and the Cloud";
            Description = "One day we woke up and things were different. Maybe it happened overnight, maybe it took many years. Suddenly the ASP.NET Web Stack is open source, hosted using Git on CodePlex and taking pull requests from the Mono team. We can run node.js and Java alongside ASP.NET in the Azure Cloud and deploy them easily. The Visual Studio editor supports HTML5, CSS3 and JavaScript in a big way. ASP.NET ships not only the open source jQuery library out of the box but also KnockoutJS, jQuery UI, jQuery Mobile and Modernizr. The Azure SDKs are hosted on Github. We are scripting thousands of Virtual Machines from the command line while others are creating things today with JavaScript that were impossible yesterday. Join Scott Hanselman as he explores the relationship between the Cloud and the Browser, many Languages and one Languages, how it might all fit together and what comes next. Development is fun again.";

            var xamlText = XAMLConverter.HtmlToXamlConverter.ConvertHtmlToXaml(Description, false);
            var xamlDescription = XamlReader.Parse(xamlText);
            var flowDoc = new FlowDocument();
            if (xamlDescription is Section section)
            {
                section.FontFamily = new FontFamily("Segoe UI");
                section.FontSize = 16;
                section.Margin = new Thickness(0, 0, 0, 0);
                flowDoc.Blocks.Add(section);
            }
            FlowDocDescription = flowDoc;
            EventName = "Build";
            EventYear = "2012";
            Link = "http://channel9.msdn.com/Events/Build/2012/3-027";
            SessionType = "Theater";
            SlidesLink = "http://video.ch9.ms/sessions/build/2012/3-027.pptx";
            Code = "3-027";
            Level = "300 - Advanced";
            Thumbnailimage = "http://video.ch9.ms/sessions/build/2012/3-027.jpg";
            Starts = DateTime.Parse("11/1/2012 8:30:00 AM");
            Finish = DateTime.Parse("11/1/2012 9:30:00 AM");

            Categories = new List<ICategory>()
            {
                new Category() { Name = "Code" },
                new Category() { Name = ".Net" },
                new Category() { Name = "Code" },
                new Category() { Name = ".Net" },
                new Category() { Name = "Code" },
                new Category() { Name = ".Net" },
                new Category() { Name = "Code" },
                new Category() { Name = ".Net" }
            };

            Speakers = new List<ISpeaker>()
            {
                new Speaker() { Name = "Enim blandit" },
                new Speaker() { Name = "Augue conubia hendrerit" },
                new Speaker() { Name = "Enim blandit" },
                new Speaker() { Name = "Augue conubia hendrerit" },
                new Speaker() { Name = "Enim blandit" },
                new Speaker() { Name = "Augue conubia hendrerit" },
                new Speaker() { Name = "Enim blandit" },
                new Speaker() { Name = "Augue conubia hendrerit" }
            };

            Media = new List<IMedia>()
            {
                new Media() { DownloadLink = "Curabitur aenean vestibulum class mauris", MediaType = "Phasellus curae adipiscing", SessionId = 35, IsDownloaded = true },
                new Media() { DownloadLink = "Curabitur aenean vestibulum class mauris", MediaType = "Phasellus curae adipiscing", SessionId = 35, IsDownloadInProgress = true },
                new Media() { DownloadLink = "Curabitur aenean vestibulum class mauris", MediaType = "Phasellus curae adipiscing", SessionId = 35, IsPlayableInMediaElement = true }
            };
        }

        public FlowDocument FlowDocDescription { get; set; }
    }
}
