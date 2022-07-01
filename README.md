# Channel Nine Event Viewer

### UPDATE: [Channel 9 is now part of Microsoft Learn](https://docs.microsoft.com/en-us/teamblog/learntvannouncement).  This application no longer works because the events RSS feed Url is no longer available.
### Please go to https://docs.microsoft.com/en-us/events to be able to locate old Event content.
### This project will be decommissioned.


A Channel 9 Event RSS feed service interpreter that stores feed data locally and has a simple user interface for viewing or linking to presentation media.  Useful for downloading videos for offline viewing.  Configurable to support most events listed at https://channel9.msdn.com/Browse/Events.

#### Basic Use
 - <b>Required</b>: Visual Studio 2017 for Windows (Community or higher)
 - Open the solution and set the startup project to ChannelNineEventFeed.WPF
 - Build
 - Start Debugging 
 - Select from a preset event filter
 - Select a year to begin downloading event data
 - Filter by categories, or by speakers
 - Click on the details button for a presentation to display a summary
 - Click on a media link to display a built in video player <i>(NOTE: The player may stall when beginning playback and has other bugs to be fixed)</i>
 - Click on a speaker link to navigate to a web page for the speaker
 - Click on a presentation link to navigate to a web page for the event and session
 - Videos can be downloaded to your current Windows user "My Videos" directory.  <i>(NOTE: If that default directory location was moved, the code currently may not be able to locate the correct folder.)</i>

#### Advanced Use
 - Default event configuration, event names, and years (or secondary feed routes), is stored in the file eventData.json.  The file is in the project ChannelNineEventFeed.Data in the Events folder.
	- You can add events to this file as needed by inspecting the root links to events listed on the https://channel9.msdn.com/Browse/Events page.  The pattern is simple: https&#58;//channel9.msdn.com/Events/<b>[Event Name]</b>/<b>[Event Year]</b>
 - The folder location for video downloads is determined at runtime.  The code for that is location in the ChannelNineEventFeed.WPF project in the MediaWindow.xaml.cs and PresentationWindow.xaml.cs files.

