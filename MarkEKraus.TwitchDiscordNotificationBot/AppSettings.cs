using System;
using System.Collections.Generic;

namespace MarkEKraus.TwitchDiscordNotificationBot
{
    internal class AppSettings
    {
        public string TwitchApiClientId { get; set; }
        public string TwitchApiClientSecret { get; set; }
        public int TwitchApiCheckIntervalSeconds { get; set; } = 60;
        public IList<TwitchChannel> TwitchChannels { get; set; }
        public Uri DiscordWebHookUri { get; set; }
        public bool EnableConsoleLogging { get; set; } = false;
        public bool EnableFileLogging { get; set; } = true;
    }
}