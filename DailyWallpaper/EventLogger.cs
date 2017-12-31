using System.Diagnostics;

namespace DailyWallpaper
{
    public class EventLogger
    {
        public static void LogEvent(string message)
        {
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "DailyWallpaper App";
                eventLog.WriteEntry(message, EventLogEntryType.Error);
            }
        }
    }
}
