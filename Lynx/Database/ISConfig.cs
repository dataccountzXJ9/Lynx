using System.Collections.Generic;
using Discord;
using Discord.WebSocket;
using System;
using Discord.Commands;
namespace Lynx.Database
{    
    public class SConfig
    { 
        public string Id { get; set; }
        public string ServerPrefix { get; set; }
        public EmbedB WelcomeMessage { get; set; } = new EmbedB();
        public EmbedB LeaveMessage { get; set; } = new EmbedB();
        public EventsWrapper Events { get; set; } = new EventsWrapper();
        public ModerationWrapper Moderation { get; set; } = new ModerationWrapper();
    }
    public class EventsWrapper
    {
        public bool LogState { get; set; } = false;
        public string LogChannel { get; set; } = "0";
        public EventWrapper Join { get; set; } = new EventWrapper();
        public EventWrapper Leave { get; set; } = new EventWrapper();
        public bool PresenceUpdate { get; set; } = true;
        public bool StatusPresenceUpdate { get; set; } = false;
        public bool UserBan { get; set; } = true;
        public bool UserUnban { get; set; } = true;
        public bool VoicePresenceUpdate { get; set; } = true;
        public bool MessageUpdate { get; set; } = true;
        public bool MessageDelete { get; set; } = true;
        public bool ChannelDelete { get; set; } = true;
        public bool ChannelUpdate { get; set; } = true;
        public bool ChannelCreate { get; set; } = true;
        public bool RoleCreate { get; set; } = true;
        public bool RoleUpdate { get; set; } = true;
        public bool RoleDelete { get; set; } = true;
        public bool NSFWWarning { get; set; } = true;
    }
    public class EmbedB
    {
        public string Title { get; set; } 
        public string Description { get; set; }
        public string URL { get; set; }
        public string ThumbnailURL { get; set; }
        public string ImageURL { get; set; }
        public uint ColorHex { get; set; }
        public EmbedAuthorBuilder WithAuthor { get; set; } = new EmbedAuthorBuilder();
        public EmbedFooterBuilder WithFooter { get; set; } = new EmbedFooterBuilder();
    }
    public class EmbedAuthorBuilder
    {
        public string Name { get; set; }
        public string IconUrl { get; set; }
    }
    public class EmbedFooterBuilder
    {
        public string Text { get; set; }
        public string IconUrl { get; set; }
    }
    public class EventWrapper
    {
        public bool IsEnabled { get; set; }
        public string TextChannel { get; set; } = "0";
    }
    public class ModerationWrapper
    {
        public string MuteRoleID { get; set; } = "0";
        public RoleWrapper DefaultAssignRole { get; set; } = new RoleWrapper();
        public List<string> AssignableRoles { get; set; } = new List<string>();
    }
    public class RoleWrapper
    {
        public bool AutoAssignEnabled { get; set; }
        public string AssignRoleID { get; set; } = "0";
    }
    public class MuteWrapper
    {
        public string GuildId { get; set; }
        public string Reason { get; set; }
        public string ModeratorId { get; set; } = "0";
        public DateTime MutedAt { get; set; }
        public DateTime UnmuteTime { get; set; }
    }
    public class GuildMuteList
    {
        public string Id { get; set; }
        public Dictionary<string, MuteWrapper> MuteList { get; set; } = new Dictionary<string, MuteWrapper>();
    }
}
